using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handle viewing of image
namespace CylinderMenu {
	public class WebcamViewer : MonoBehaviour {

		public void ViewWebcam (Texture2D image) {
			gameObject.SetActive(true);

			InputManager.instance.ToggleViewMode();
			InputManager.instance.goDown.AddListener(HideWebcam);
		}

		public void HideWebcam () {
			InputManager.instance.goDown.RemoveListener(HideWebcam);
			InputManager.instance.ToggleViewMode();

			gameObject.SetActive(false);

			MenuManager.instance.ExitWebcam();
		}
	}
}
