﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
namespace CylinderMenu {
	public class ImageViewer : MonoBehaviour {
		//public Vector3 picContainerOffset;

		public float[] zoomLevels;

		private Vector3 bottomLeftView;
		private Vector3 topRightView;
		private Vector3 extent;
		private Vector2 clampHigh;
		private Vector2 clampLow;

		private bool viewingImage;
		private Material mat;

		private int currentZoom = 0;

		// Use this for initialization
		void Awake () {
			mat = GetComponent<Renderer>().material;
			gameObject.SetActive(false);
		}

		void Update () {
			if (viewingImage) {
				// Scroll around the image when the looking around
				Vector3 inputMovement = InputManager.instance.Get2DMovement() * 0.2f;
				Vector3 nextMove = transform.position + inputMovement;

				nextMove.x = Mathf.Clamp(nextMove.x, clampHigh.x, clampLow.x);
				nextMove.y = Mathf.Clamp(nextMove.y, clampHigh.y, clampLow.y);

				transform.position = nextMove;
			}
		}


		public void ViewImage (Texture image) {
			InputManager.instance.ToggleViewMode();
			InputManager.instance.goDown.AddListener(ZoomOut);
			InputManager.instance.goUp.AddListener(ZoomIn);

			mat.mainTexture = image;

			transform.position = new Vector3(0f, 15f, 0f);
			SetZoomLevel();


			gameObject.SetActive(true);
			StartCoroutine(InputGracePeriod());
		}

		public void SetZoomLevel () {
			transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + zoomLevels[currentZoom]);

			// Set up image clamp so the user cannot scroll beyond the boundary of the image
			// Could be called in Awake - unless the camera ever moves or the size of the Quad the image is printed on changes
			bottomLeftView = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, zoomLevels[currentZoom]));
			topRightView = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, zoomLevels[currentZoom]));
			extent = GetComponent<Renderer>().bounds.extents;
			clampHigh = new Vector2(topRightView.x - extent.x, topRightView.y - extent.y);
			clampLow = new Vector2(bottomLeftView.x + extent.x, bottomLeftView.y + extent.y);
		}

		public void ZoomOut () {
			currentZoom--;
			if (currentZoom < 0) {
				currentZoom = 0;
				HideImage();
			} else {
				SetZoomLevel();
			}
		}

		public void ZoomIn () {
			currentZoom++;
			if (currentZoom >= zoomLevels.Length) {
				currentZoom = zoomLevels.Length - 1;
			} else {
				SetZoomLevel();
			}

		}

		private void HideImage () {
			InputManager.instance.ToggleViewMode();
			InputManager.instance.goDown.RemoveListener(ZoomOut);
			InputManager.instance.goUp.RemoveListener(ZoomIn);
			viewingImage = false;
			StartCoroutine(ToggleActiveAfterSeconds(0.5f));

			MenuManager.instance.ExitImageView();
		}


		private IEnumerator ToggleActiveAfterSeconds (float t) {
			yield return new WaitForSeconds(t);
			gameObject.SetActive(!gameObject.activeSelf);
		}

		private IEnumerator InputGracePeriod () {
			yield return new WaitForSeconds(0.5f);
			viewingImage = true;
		}



		/*
		private IEnumerator HideAnimation()
		{
			Vector3 end = picContainerOffset;
			end.z = 9f;
			yield return StartCoroutine(ZoomAnimation(0.5f, transform.position, end, new Vector3(15f, 15f, 1f), new Vector3(5.87f, 4.8f, 1f)));

			mat.mainTexture = null;
			gameObject.SetActive(false);
			transform.localScale = new Vector3(15f, 15f, 1f);
		}
	
		private IEnumerator DeactivateAfter(float t)
		{
			yield return new WaitForSeconds(t);
			mat.mainTexture = null;
			gameObject.SetActive(false);
		}

		private IEnumerator ZoomAnimation(float animTime, Vector3 startPos, Vector3 endPos, Vector3 startScale, Vector3 endScale)
		{
			transform.position = startPos;
			transform.localScale = startScale;
			float t = 0;

			while (t < 1) {
				t += Time.deltaTime / animTime;

				float tEased = Easing.Quadratic.Out(t);
				tEased = t;
				transform.position = Vector3.Lerp(startPos, endPos, tEased);
				transform.localScale = Vector3.Lerp(startScale, endScale, tEased);

				yield return null;
			}
		}

		private IEnumerator ZoomAnimation(float animTime, Vector3 startPos, Vector3 endPos)
		{
			transform.position = startPos;
			float t = 0;

			while (t < 1)
			{
				t += Time.deltaTime / animTime;

				float tEased = Easing.Quadratic.Out(t);
				tEased = t;
				transform.position = Vector3.Lerp(startPos, endPos, tEased);

				yield return null;
			}
		}
		*/

	}
}
