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

    private Coroutine updateTimer;

    // Use this for initialization
    void Start () {
        RayCaster.instance.OnRayEnter += RayEnterHandler;
        RayCaster.instance.OnRayExit += RayExitHandler;
        RayCaster.instance.looker = Camera.main.transform;
        updateTimer = StartCoroutine(SetTimer());
    }

    private void Increment(float by, ref int val)
    {
        if (by > 5)
            val += 5;
        else if (by > 1)
            val += 1;
    }

    private void Decrement(float by, ref int val)
    {
        if (by > 5)
            val -= 5;
        else if (by > 1)
            val -= 1;
    }

    private IEnumerator SetTimer()
    {
        while (true)
        {
            Debug.Log("Updating Timers");
            Increment(sUpTime, ref secs);
            Decrement(sDownTime, ref secs);
            Increment(minUpTime, ref mins);
            Decrement(minDownTime, ref mins);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void RayEnterHandler(GameObject hit)
    {
        switch (hit.name)
        {
            case "Seconds Up":
                sUpTime = 0f;
                break;
            case "Seconds Down":
                sDownTime = 0f;
                break;
            case "Minutes Up":
                minUpTime = 0f;
                break;
            case "Minutes Down":
                minDownTime = 0f;
                break;
        }
    }

    public void RayExitHandler(GameObject hit)
    {
        switch (hit.name)
        {
            case "Seconds Up":
                sUpTime = -1f;
                break;
            case "Seconds Down":
                sDownTime = -1f;
                break;
            case "Minutes Up":
                minUpTime = -1f;
                break;
            case "Minutes Down":
                minDownTime = -1f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RayCaster.instance.CastForward();
        // Second Controls
        if (sUpTime >= 0)
            sUpTime += Time.deltaTime;
        else if (sDownTime >= 0)
            sDownTime += Time.deltaTime;
        else if (minUpTime >= 0)
            minUpTime += Time.deltaTime;
        else if (minDownTime >= 0)
            minDownTime += Time.deltaTime;

        minText.text = mins.ToString();
        secText.text = secs.ToString();
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
