using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayTransitioner : MonoBehaviour {

    public OverlayTransitioner instance { get; private set; }

    ScreenType currentScreen = ScreenType.MainMenu;
    public List<Image> overlays;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public void TransitionTo(ScreenType newScreen)
    {
        StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime, overlays[(int)currentScreen], overlays[(int)newScreen]));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime, Image imageOut, Image imageIn)
    {
        float t = 0;
        float alpha = startAlpha;
        Color col = imageOut.color;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
            col.a = alpha;
            imageOut.color = col;
            yield return null;
        }
        if (imageIn != null)
        {
            imageOut.gameObject.SetActive(false);
            imageIn.gameObject.SetActive(true);
            StartCoroutine(Fade(0f, 1f, 0.3f, imageIn, null));
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
    ImageMenu = 1,
    PIPDisplay = 2,
    ImageViewer = 3 ,
    CameraDisplay = 4,
    Timer = 5,
    Settings = 6,
}