using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handle viewing of image
namespace CylinderMenu {
	public class WebcamViewer : TransitionableObject
	{

		public CanvasGroup canvasGroup;

		override protected IEnumerator TransitionIn ()
		{
			OverlayTransitioner.instance.TransitionIn(ScreenType.CameraDisplay);

			InputManager.instance.ToggleViewMode();

			yield return Fade(0f, 1f, Constants.Transitions.FadeTime);

			InputManager.instance.goDown.AddListener(StartTransitionOut);
		}

		override protected IEnumerator TransitionOut ()
		{
			InputManager.instance.goDown.RemoveListener(StartTransitionOut);

			yield return Fade(1f, 0f, Constants.Transitions.FadeTime);

			InputManager.instance.ToggleViewMode();
			gameObject.SetActive(false);
			MenuManager.instance.ExitWebcam();
		}

		private IEnumerator Fade(float from, float to, float totalTime)
		{
			float t = 0;
			float currentT = 0;

			while (t < totalTime) {
				t += Time.deltaTime;
				currentT = Easing.Quadratic.Out(t / totalTime);
				canvasGroup.alpha = Mathf.Lerp(from, to, currentT);
				yield return null;
			}
		}
	}
}
