using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionableObject : MonoBehaviour {

	public void StartTransitionIn () {
		gameObject.SetActive(true);
		StartCoroutine(TransitionIn());
	}

	public void StartTransitionOut () {
		StartCoroutine(TransitionOut());
	}

	virtual protected IEnumerator TransitionIn() {
		yield return null;
	}

	virtual protected IEnumerator TransitionOut () {
		yield return null;
	}

}
