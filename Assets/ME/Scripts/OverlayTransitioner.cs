using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayTransitioner : MonoBehaviour
{

    public OverlayTransitioner instance { get; private set; }

    ScreenType currentScreen = ScreenType.MainMenu;
    public float fadeTime = 0.3f;
    public Text menuTitle;
    public List<Image> overlays;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    // Transition between systems - From an IFadeable TO and IFadeable.
    // transType: Type of transition (MenuToMenu, MenuToSystem, SystemToMenu)
    // screenType: Destination overlay
    public IEnumerator Transition(IFadeable from, IFadeable to, TransitionType transType, ScreenType screenType)
    {
        switch (transType)
        {
            case TransitionType.MenuToMenu:
                // Fade out Menu Title and Items
                StartCoroutine(Fade(1f, 0f, fadeTime, menuTitle));
                yield return from.TransitionOut(fadeTime);
                menuTitle.text = (to as CylinderMenu.MenuRow).name;
                // Fade in Menu Title
                StartCoroutine(Fade(0f, 1f, fadeTime, menuTitle));
                break;

            case TransitionType.MenuToSystem:
                // Fade out Menu Title, Overlay, and Items
                StartCoroutine(Fade(1f, 0f, fadeTime, menuTitle));
                StartCoroutine(Fade(1f, 0f, fadeTime, overlays[(int)currentScreen]));
                yield return from.TransitionOut(fadeTime);
                // Fade in Overlay
                currentScreen = screenType;
                StartCoroutine(Fade(0f, 1f, fadeTime, overlays[(int)currentScreen]));
                break;

            case TransitionType.SystemToMenu:
                // Fade out Overlay and System
                StartCoroutine(Fade(1f, 0f, fadeTime, overlays[(int)currentScreen]));
                yield return from.TransitionOut(fadeTime);
                // Fade in Menu Title, Overlay
                currentScreen = screenType;
                StartCoroutine(Fade(0f, 1f, fadeTime, menuTitle));
                StartCoroutine(Fade(0f, 1f, fadeTime, overlays[(int)currentScreen]));
                break;

            default:
                Debug.Log("Bad transition");
                break;
        }
        // Fade in system/items
        to.TransitionIn(fadeTime);
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
    IEnumerator TransitionOut(float fadeTime);
}

public enum TransitionType
{
    MenuToMenu,
    MenuToSystem,
    SystemToMenu,
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