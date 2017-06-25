using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class MenuItem : TransitionableObject
	{
		// Object Links
		public GameObject PicPrefab;
		private Collider col;
		protected Transform selectors;

		[Space(10)]

		// Parameters
		public float hoverForward;
		public float hoverTime = 2f;
		public string itemName;
		public Texture menuPic;

		protected Transform pic;

		// Runtime variables
		private float zDistance;
		private float zDistanceHovering;

		// These 3 should be in TransitionItem but to simplify some code in MenuManager I'm leaving them here for now
		public MenuManager.SelectAction selectAction;
		public Vector3 rowItemSize = new Vector3(4f, 4f, 4f);
		[HideInInspector]
		public Texture FullSizedPic;
		[HideInInspector]
		public List<MenuItem> subMenuItems; // Explicit reference to the submenu transform so we can use it in dynamic menu creation

		// Initialize List
		virtual protected void Awake ()
		{
			subMenuItems = new List<MenuItem>();

			// Get each MenuItem from the SubMenu container
			foreach (Transform sub in transform) {
				subMenuItems.Add(sub.GetComponent<MenuItem>());
			}

			col = GetComponent<Collider>();

			// Instantiate the mesh that holds the menu picture
			pic = Instantiate(PicPrefab, transform).transform;
			pic.Find("mesh").GetComponent<Renderer>().material.mainTexture = menuPic;

			selectors = new GameObject().transform;
			selectors.parent = transform;

			selectors.gameObject.SetActive(false);
			gameObject.SetActive(false);
		}

		virtual public void AddToMenuRow (Transform row, float distance, Quaternion rotation, Vector3 scale)
		{
			transform.SetParent(row);
			transform.localPosition = Vector3.zero;
			transform.localRotation = rotation;

			BoxCollider col = GetComponent<BoxCollider>();
			col.center = new Vector3(0f, 0f, distance);
			col.size = new Vector3(scale.x, scale.y, 0.1f);

			zDistance = distance;
			zDistanceHovering = distance - hoverForward;

			pic.localPosition = new Vector3(0f, 0f, zDistance);
			pic.localScale = new Vector3(scale.x, scale.y, scale.z);

			selectors.localPosition = new Vector3(0f, 0f, distance);

			gameObject.SetActive(true);
		}

		virtual public void LookAt () {
			StartCoroutine(SmoothMovement(new Vector3(0f, 0f, zDistanceHovering)));
			selectors.gameObject.SetActive(true);
		}

		virtual public void LookAway () {
			StartCoroutine(SmoothMovement(new Vector3(0f, 0f, zDistance)));
			selectors.gameObject.SetActive(false);
		}

		private IEnumerator SmoothMovement (Vector3 end) {
			Vector3 startPos = pic.transform.localPosition;
			float t = 0;

			while (t < 1) {
				t += Time.deltaTime / hoverTime;
				t = Mathf.Clamp01(t);

				Vector3 newPos = Vector3.Lerp(startPos, end, t);
				pic.transform.localPosition = newPos;
				selectors.localPosition = new Vector3(0f, selectors.localPosition.y, newPos.z);
				yield return null;
			}
		}


		// Transitions

		override protected IEnumerator TransitionOut () {
			col.enabled = false;
			yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));
			// Nothing after
		}

		override protected IEnumerator TransitionIn () {
			col.enabled = true;
			yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
			// Nothing after
		}

		private IEnumerator Fade (float startAlpha, float endAlpha, float totalTime) {
			Material mat1 = pic.Find("mesh").GetComponent<Renderer>().materials[0];

			float t = 0;
			float alpha = startAlpha;

			while (t < totalTime) {
				t += Time.deltaTime;
				alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
				mat1.SetFloat("_Alpha", alpha);
				yield return null;
			}
		}
	}

}
