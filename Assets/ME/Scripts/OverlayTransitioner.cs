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

    public void TransitionTo(ScreenType newScreen, bool toggleView)
    {
        StartCoroutine(Fade(1f, 0f, 0.3f, overlays[0], overlays[(int)newScreen], toggleView));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime, Image imageOut, Image imageIn, bool toggleView)
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
            if(toggleView)
                InputManager.instance.ToggleViewMode();
            StartCoroutine(Fade(0f, 1f, 0.3f, imageIn, null, false));
        }
    }
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