using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScreen : TransitionableObject {

    [SerializeField]
    private Text minText, secText;

    private int mins = 0;
    private int secs = 0;

	public float baseTimeToProc = 1f;
	public float minTimeToProc = 0.1f;
	public float timeLessPerProc = 0.1f;
	public float timeToProc = 1f;
	public float currentTime = 0f;
	public int consecutiveProcs = 0;

    private Coroutine updateTimer;

    // Use this for initialization
    void Start ()
	{
        RayCaster.instance.OnRayStay += RayStayHandler;
		RayCaster.instance.OnRayExit += RayExitHandler;
		RayCaster.instance.looker = Camera.main.transform;
		RayCaster.instance.StartRaycasting();

		currentTime = timeToProc;
	}

	public void RayStayHandler(GameObject hit)
	{
		currentTime += Time.deltaTime;
		if (currentTime >= timeToProc) {
			currentTime = 0f;
			timeToProc = Mathf.Max(timeToProc - timeLessPerProc, minTimeToProc);

			switch (hit.name) {
				case "Seconds Up":
					secs++;
					if (secs >= 60) {
						if (mins < 999) {
							secs -= 60;
							mins++;
						} else {
							secs = 59;
						}
					}
					break;
				case "Seconds Down":
					secs--;
					if (secs < 0) {
						if (mins > 0) {
							secs += 60;
							mins--;
						} else {
							secs = 0;
						}
					}
					break;
				case "Minutes Up":
					mins = Mathf.Min(mins + 1, 999);
					break;
				case "Minutes Down":
					mins = Mathf.Max(mins - 1, 0);
					break;
			}
			UpdateText();
		}
	}


    public void RayExitHandler(GameObject hit)
    {
		timeToProc = baseTimeToProc;
		currentTime = timeToProc;
	}

	private void UpdateText () {
		secText.text = secs < 10 ? "0" + secs.ToString() : secs.ToString();
		minText.text = mins < 10 ? "0" + mins.ToString() : mins.ToString();
	}

	public void Back () {
		StartTransitionOut(CylinderMenu.MenuManager.instance);
	}

	override protected IEnumerator TransitionIn()
    {
		RayCaster.instance.OnRayStay += RayStayHandler;
		RayCaster.instance.OnRayExit += RayExitHandler;
		RayCaster.instance.looker = Camera.main.transform;
		RayCaster.instance.StartRaycasting();

		yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));

		InputManager.instance.goDown.AddListener(Back);
	}

	protected override IEnumerator TransitionOut ()
	{
		InputManager.instance.goDown.RemoveListener(Back);
		RayCaster.instance.StopRaycasting();
		RayCaster.instance.OnRayStay -= RayStayHandler;
		RayCaster.instance.OnRayExit -= RayExitHandler;

		yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));
	}

	private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        yield return null;
    }
}
