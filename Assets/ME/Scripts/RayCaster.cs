using System;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster {

	public Transform looker;

	public event Action<GameObject> OnRayEnter;
	public event Action<GameObject> OnRayStay;
	public event Action<GameObject> OnRayExit;

	private List<RayCasterHit> currentHits;

	public RayCaster() {
		currentHits = new List<RayCasterHit>();
	}

	public void CastForward() {

		// Reset 'checked' flag
		currentHits.ForEach(h => { h.checkedYet = false; });

		// Cast ray forward
		Ray ray = new Ray(looker.position, looker.forward);
		RaycastHit[] hits = Physics.RaycastAll(ray);

		foreach (RaycastHit hit in hits) {

			// Check each ray hit to see if it is currently being hit already
			RayCasterHit match = LookForMatch(hit.transform.gameObject);
			if (match == null) {
				// Ray Enter
				currentHits.Add(new RayCasterHit(hit.transform.gameObject));

				OnEnter(hit.transform.gameObject);
			} else {
				match.checkedYet = true;

				// Ray Stay
				OnStay(match.obj);
			}
		}

		// Check the rest of the current hits and trigger their exit
		for (int i = currentHits.Count - 1; i >= 0; --i) {
			if (currentHits[i].checkedYet == false) {
				// Ray Exit
				if (currentHits[i].obj != null)
					OnExit(currentHits[i].obj);

				currentHits.RemoveAt(i);
			}
		}

		#if UNITY_EDITOR
			Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red);
		#endif
	}

	RayCasterHit LookForMatch (GameObject obj) {
		foreach (RayCasterHit h in currentHits) {
			if (h.obj == obj) {
				return h;
			}
		}

		return null;
	}

	private void OnEnter(GameObject o) {
		OnRayEnter(o);
	}

	private void OnStay (GameObject o) {
		OnRayStay(o);
	}

	private void OnExit (GameObject o) {
		OnRayExit(o);
	}
}

public class RayCasterHit {
	public GameObject obj;
	public bool checkedYet = true;

	public RayCasterHit (GameObject h) {
		obj = h;
	}
}