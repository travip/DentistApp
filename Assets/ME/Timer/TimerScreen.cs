using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScreen : TransitionableObject {

    public static TimerScreen instance { get; private set; }

    [SerializeField]
    private List<Renderer> rends;

    public Text minText, secText;

    private int mins = 0;
    private int secs = 0;

	public float baseTimeToProc = 1f;
	public float minTimeToProc = 0.1f;
	public float timeLessPerProc = 0.1f;
	public float timeToProc = 1f;
	public float currentTime = 0f;
	public int consecutiveProcs = 0;

    public TimerSelector startTime, stopTime;

    void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization
    void Start ()
	{
		RayCaster.instance.OnRayEnter += RayEnterHandler;
		RayCaster.instance.OnRayStay += RayStayHandler;
		RayCaster.instance.OnRayExit += RayExitHandler;
		RayCaster.instance.looker = Camera.main.transform;
		RayCaster.instance.StartRaycasting();

		currentTime = timeToProc;
	}

	public void RayEnterHandler(GameObject hit)
	{
		if (hit.tag == "TimerButton") {
			hit.GetComponent<Renderer>().material.SetColor("_Tint", Color.green);
		}
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
		if (hit.tag == "TimerButton") {
			hit.GetComponent<Renderer>().material.SetColor("_Tint", Color.white);
		}

		timeToProc = baseTimeToProc;
		currentTime = timeToProc;
	}

	private void UpdateText () {
		secText.text = secs < 10 ? "0" + secs.ToString() : secs.ToString();
		minText.text = mins < 10 ? "0" + mins.ToString() : mins.ToString();
	}

    public void BeginTimer()
    {
        CylinderMenu.GlobalTimer.instance.StartTimer(mins * 60 + secs);
    }

    public void StopTimer()
    {
        CylinderMenu.GlobalTimer.instance.ResetTimer();
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
        startTime.gameObject.SetActive(true);
        stopTime.gameObject.SetActive(true);

        InputManager.instance.goDown.AddListener(Back);
	}

	protected override IEnumerator TransitionOut ()
	{
		InputManager.instance.goDown.RemoveListener(Back);
		RayCaster.instance.StopRaycasting();
		RayCaster.instance.OnRayStay -= RayStayHandler;
		RayCaster.instance.OnRayExit -= RayExitHandler;
        startTime.gameObject.SetActive(false);
        stopTime.gameObject.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        float t = 0;
        float alpha = startAlpha;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
            foreach(Renderer r in rends)
                r.materials[0].SetFloat("_Alpha", alpha);
            yield return null;
        }
    }
}
