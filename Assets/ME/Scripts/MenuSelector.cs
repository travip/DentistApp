using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CylinderMenu {

	public class MenuSelector : MonoBehaviour {

		public enum SelectionType
		{
			select,
			back,
			nextPage,
			previousPage
		}

		private Transform imagesTransform;

		public SelectionType selectionType;
		public float totalTime = 1f;
		private float currentTime = 0f;
		public MenuItem parentMenuItem;

		public Transform scalingImage;
		public float minScale = 1f;
		public float maxScale = 3.1f;
		

		public float reduceTimeMultiplier = 1f;

		public float selectedZ;
		public float selectMoveSpeed;

		private bool hitThisFrame;
		private float targetZ;
		private SelectionSpinner spinner;

		void Start() {
			if (parentMenuItem == null && transform.parent != null) {
				parentMenuItem = transform.parent.GetComponent<MenuItem>();
			}
			imagesTransform = transform.Find("Images");
			targetZ = 0f;
			selectedZ = -selectedZ;
			spinner = GetComponent<SelectionSpinner>();
		}

		public bool LookAt () {
			targetZ = selectedZ;
			if (spinner != null)
				spinner.selecting = true;

			hitThisFrame = true;
			currentTime += Time.deltaTime;
			SetInsideSize(currentTime / totalTime);

			if (currentTime > totalTime) {
				currentTime = 0f;
				NoSelection();
				return true;
			}

			return false;
		}

		private void SetInsideSize(float percent) {
			float newScale = minScale + (percent * (maxScale - minScale));
			scalingImage.localScale = new Vector3(newScale, newScale, newScale);
		}

		void Update() {
			if (currentTime > 0f && hitThisFrame == false) {
				currentTime -= Time.deltaTime * reduceTimeMultiplier;
				NoSelection();
			}

			if (imagesTransform.localPosition.z != targetZ) {
				MoveTowardsTarget();
			}

			hitThisFrame = false;
		}

		void NoSelection() {
			SetInsideSize(currentTime / totalTime);
			targetZ = 0f;
			if (spinner != null)
				spinner.selecting = false;
		}

		void MoveTowardsTarget() {
			if (imagesTransform.localPosition.z > targetZ) {
				Vector3 v = imagesTransform.localPosition;
				v.z -= Time.deltaTime * selectMoveSpeed;
				if (v.z < targetZ)
					v.z = targetZ;

				imagesTransform.localPosition = v;
			} else {
				Vector3 v = imagesTransform.localPosition;
				v.z += Time.deltaTime * selectMoveSpeed;
				if (v.z > targetZ)
					v.z = targetZ;

				imagesTransform.localPosition = v;
			}
		}

	}
}
