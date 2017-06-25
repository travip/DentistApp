using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScreen : TransitionableObject {

    [SerializeField]
    private Text minText, secText;

    private int mins = 0;
    private int secs = 0;
    private bool isTiming = false;

    [SerializeField]
    private GameObject secondsUp, secondsDown, minutesUp, minutesDown;
    public float sUpTime = -1;
    public float sDownTime = -1;
    public float minUpTime = -1;
    public float minDownTime = -1;

	public float baseTimeToProc = 1f;
	public float minTimeToProc = 0.1f;
	public float timeToProc = 1f;
	public float totalTime = 0f;
	public int consecutiveProcs = 0;

    private Coroutine updateTimer;

    // Use this for initialization
    void Start () {
        RayCaster.instance.OnRayStay += RayStayHandler;
		RayCaster.instance.OnRayExit += RayExitHandler;
		RayCaster.instance.looker = Camera.main.transform;
		RayCaster.instance.StartRaycasting();
	}

	public void RayStayHandler(GameObject hit)
	{
		totalTime += Time.deltaTime;
		if (totalTime >= timeToProc) {
			totalTime = 0f;
			timeToProc = Mathf.Max(timeToProc - 0.1f, minTimeToProc);

			switch (hit.name) {
				case "Seconds Up":
					secs++;
					break;
				case "Seconds Down":
					secs--;
					break;
				case "Minutes Up":
					mins++;
					break;
				case "Minutes Down":
					mins--;
					break;
			}
		}
	}

    public void RayExitHandler(GameObject hit)
    {
		totalTime = 0f;
		timeToProc = baseTimeToProc;
	}

	override protected IEnumerator TransitionIn()
    {
        yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        yield return null;
    }
}
