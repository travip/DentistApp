using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CylinderMenu {

	public class MenuSelector : MonoBehaviour {

		public float totalTime = 1f;
		private float currentTime = 0f;
		public MenuItem parentMenuItem;

		public Transform insideImage;
		public float minSize = 1f;
		public float maxSize = 3.1f;

		public float reduceTimeMultiplier = 1f;

		private bool hitThisFrame;

		void Start() {
			if (parentMenuItem == null) {
				parentMenuItem = transform.parent.GetComponent<MenuItem>();
			}
		}

		public bool LookAt () {
			hitThisFrame = true;
			currentTime += Time.deltaTime;
			setInsideSize(currentTime / totalTime);

			if (currentTime > totalTime) {
				currentTime = 0f;
				setInsideSize(0f);
				return true;
			}

			return false;
		}

		private void setInsideSize(float percent) {
			float newScale = minSize + (percent * (maxSize - minSize));
			insideImage.localScale = new Vector3(newScale, newScale, newScale);
		}

		void Update() {
			if (currentTime > 0f && hitThisFrame == false) {
				currentTime -= Time.deltaTime * reduceTimeMultiplier;
				setInsideSize(currentTime / totalTime);
			}

			hitThisFrame = false;
		}
	}
}
