using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayTransitioner : MonoBehaviour
{

    public OverlayTransitioner instance { get; private set; }

    ScreenType currentScreen = ScreenType.MainMenu;
    public Text menuTitle;
    public List<Image> overlays;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public IEnumerator TransitionScreen(ScreenType newScreen)
    {
        if (newScreen != ScreenType.MainMenu)
            StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime, menuTitle));      
        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime, overlays[(int)currentScreen]));

        overlays[(int)currentScreen].gameObject.SetActive(false);
        overlays[(int)newScreen].gameObject.SetActive(true);

        if(newScreen == ScreenType.MainMenu)
            StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime, menuTitle));
        StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime, overlays[(int)newScreen]));

        currentScreen = newScreen;
    }

    public IEnumerator TransitionMenuTitle(string title)
    {
        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime, menuTitle));
        menuTitle.text = title;
        StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime, menuTitle));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime, Graphic image)
    {
        float t = 0;
        float alpha = startAlpha;
        Color col = image.color;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
            col.a = alpha;
            image.color = col;
            yield return null;
        }
    }
}

public interface IFadeable
{
    void TransitionIn(float fadeTime);
    IEnumerator TransitionOut(float fadeTime, IFadeable to);
}

public enum ScreenType
{
    MainMenu = 0,
    PIPDisplay = 1,
    ImageViewer = 2 ,
    CameraDisplay = 3,
    Timer = 4,
    Settings = 5,
}