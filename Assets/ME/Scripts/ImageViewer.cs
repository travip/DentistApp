using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
namespace CylinderMenu
{
	public class ImageViewer : TransitionableObject
	{

		//public Vector3 picContainerOffset;

		public float[] zoomLevels;

		private Vector3 bottomLeftView;
		private Vector3 topRightView;
		private Vector3 extent;
		private Vector2 clampHigh;
		private Vector2 clampLow;

		private bool viewingImage = false;
		private Material mat;

		private int currentZoom = 0;

		// Use this for initialization
		void Awake ()
		{
			mat = GetComponent<Renderer>().material;
			gameObject.SetActive(false);
		}

		private IEnumerator ViewerUpdate () {

			while (viewingImage) {
				// Scroll around the image when the looking around
				Vector3 inputMovement = new Vector3();
				inputMovement.x = InputManager.instance.Get2DMovement().y;
				inputMovement.y = -InputManager.instance.Get2DMovement().x;
				inputMovement *= 0.2f;

				Vector3 nextMove = transform.position + inputMovement;

				nextMove.x = Mathf.Clamp(nextMove.x, clampHigh.x, clampLow.x);
				nextMove.y = Mathf.Clamp(nextMove.y, clampHigh.y, clampLow.y);

				transform.position = nextMove;

				yield return null;
			}
		}

		public void LoadImage(Texture image)
		{
			mat.mainTexture = image;

			transform.position = new Vector3(0f, 0f, 0f);
			currentZoom = 0;
			SetZoomLevel();
		}

		public void SetZoomLevel ()
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + zoomLevels[currentZoom]);

			// Set up image clamp so the user cannot scroll beyond the boundary of the image
			// Could be called in Awake - unless the camera ever moves or the size of the Quad the image is printed on changes
			bottomLeftView = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, zoomLevels[currentZoom]));
			topRightView = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, zoomLevels[currentZoom]));
			extent = GetComponent<Renderer>().bounds.extents;
			clampHigh = new Vector2(topRightView.x - extent.x, topRightView.y - extent.y);
			clampLow = new Vector2(bottomLeftView.x + extent.x, bottomLeftView.y + extent.y);
		}

		private void ZoomOut ()
		{
			currentZoom--;
			if (currentZoom < 0)
			{
				currentZoom = 0;
				StartTransitionOut(MenuManager.instance);
			} else
			{
				SetZoomLevel();
			}
		}

		private void ZoomIn ()
		{
			currentZoom++;
			if (currentZoom >= zoomLevels.Length) {
				currentZoom = zoomLevels.Length - 1;
			} else {
				SetZoomLevel();
			}
		}

		private void NextImage()
		{
			LoadImage(MenuManager.instance.GetNextImage());
			StartCoroutine(InputGracePeriod());
		}

		private void PreviousImage()
		{
			LoadImage(MenuManager.instance.GetPreviousImage());
			StartCoroutine(InputGracePeriod());
		}

		private IEnumerator InputGracePeriod ()
		{
			viewingImage = false;
			yield return new WaitForSeconds(0.5f);
			viewingImage = true;
			StartCoroutine(ViewerUpdate());
		}


		override protected IEnumerator TransitionIn ()
		{
			InputManager.instance.ToggleViewMode();

			yield return Fade(0f, 1f, Constants.Transitions.FadeTime);

			InputManager.instance.goDown.AddListener(ZoomOut);
			InputManager.instance.goUp.AddListener(ZoomIn);
			InputManager.instance.goLeft.AddListener(PreviousImage);
			InputManager.instance.goRight.AddListener(NextImage);

			viewingImage = true;
			SetZoomLevel();
			StartCoroutine(ViewerUpdate());
		}

		override protected IEnumerator TransitionOut (TransitionableObject inAfter)
		{
			InputManager.instance.goDown.RemoveListener(ZoomOut);
			InputManager.instance.goUp.RemoveListener(ZoomIn);
			InputManager.instance.goLeft.RemoveListener(PreviousImage);
			InputManager.instance.goRight.RemoveListener(NextImage);

			viewingImage = false;

			yield return Fade(1f, 0f, Constants.Transitions.FadeTime);

			InputManager.instance.ToggleViewMode();
			gameObject.SetActive(false);

			if (inAfter)
				inAfter.StartTransitionIn();
		}

		private IEnumerator Fade (float startAlpha, float endAlpha, float totalTime)
		{
			float t = 0;
			float alpha = startAlpha;

			while (t < totalTime) {
				t += Time.deltaTime;
				alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
				mat.SetFloat("_Alpha", alpha);
				yield return null;
			}
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
