using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionableObject : MonoBehaviour
{

	public CanvasGroup overlay;

	public void StartTransitionIn ()
	{
		gameObject.SetActive(true);

		if (overlay != null)
			StartCoroutine(FadeOverlay(0f, 1f, Constants.Transitions.FadeTime));

		StartCoroutine(TransitionIn());
	}

	public void StartTransitionOut (TransitionableObject inAfter = null)
	{
		if (overlay != null)
			StartCoroutine(FadeOverlay(1f, 0f, Constants.Transitions.FadeTime));

		StartCoroutine(TransitionOut(inAfter));
	}

	virtual protected IEnumerator TransitionIn()
	{
		yield return null;
	}

	virtual protected IEnumerator TransitionOut (TransitionableObject inAfter)
	{
		yield return null;
	}


	protected IEnumerator FadeOverlay(float startAlpha, float endAlpha, float totalTime) {
		float t = 0;
		float alpha = startAlpha;

		while (t < totalTime) {
			t += Time.deltaTime;
			alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
			overlay.alpha = alpha;
			yield return null;
		}
	}

}