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
			StartCoroutine(FadeCanvasGroup(0f, 1f, Constants.Transitions.FadeTime, overlay));

		StartCoroutine(TransitionIn());
	}

	public void StartTransitionOut (TransitionableObject inAfter = null)
	{
		if (overlay != null)
			StartCoroutine(FadeCanvasGroup(1f, 0f, Constants.Transitions.FadeTime, overlay));

		StartCoroutine(TransitionOut());

		if (inAfter != null)
			inAfter.Invoke("StartTransitionIn", Constants.Transitions.FadeTime);

		Invoke("EndTransitionOut", Constants.Transitions.FadeTime + 0.5f);
	}

	private void EndTransitionOut() {
		gameObject.SetActive(false);
		
	}

	virtual protected IEnumerator TransitionIn()
	{
		yield return null;
	}

	virtual protected IEnumerator TransitionOut ()
	{
		yield return null;
	}

	protected IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float totalTime, CanvasGroup group)
	{
		float t = 0;
		float alpha = startAlpha;

		while (t < totalTime) {
			t += Time.deltaTime;
			alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
			group.alpha = alpha;
			yield return null;
		}
	}
}