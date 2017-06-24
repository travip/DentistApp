using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyWiFi.Core;
using EasyWiFi.ServerControls;

public class PIPController : TransitionableObject, IServerController
{
    public PIPController instance { get; private set; }

    public Transform PIPPointer;

    public string control = "Gyro";
    public EasyWiFiConstants.PLAYER_NUMBER player = EasyWiFiConstants.PLAYER_NUMBER.Player1;

    //runtime variables
    GyroControllerType[] gyro = new GyroControllerType[EasyWiFiConstants.MAX_CONTROLLERS];
    int currentNumberControllers = 0;

    //values and variables
    Quaternion orientation;
    Quaternion zeroOrientation;
    Quaternion finalOrientation;

    public Transform spiritLevel;

    public float x = 0;
    public float y = 0;
    public float z = 0;
    public float w = 0;

    public float angle_x = 0;
    public float angle_y = 0;
    public float angle_z = 0;
    public float tolerance = 5;

    public float maxTolerance = 45;
    public float minTolerance = 1;

    public CanvasGroup pipCanvas;
    public RectTransform CrossNorth;
    public RectTransform CrossSouth;
    public RectTransform CrossWest;
    public RectTransform CrossEast;
    private int scaleCrossHair = 3;

    public Text sensitivity;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    void OnEnable()
    {
        Input.gyro.enabled = true;
        CrossNorth.localPosition = new Vector3(0, (110 + tolerance * scaleCrossHair), 0);
        CrossSouth.localPosition = new Vector3(0, (-110 - tolerance * scaleCrossHair), 0);
        CrossWest.localPosition = new Vector3((-110 - tolerance * scaleCrossHair), 0, 0);
        CrossEast.localPosition = new Vector3((110 + tolerance * scaleCrossHair), 0, 0);

        sensitivity.text = tolerance.ToString();

        EasyWiFiController.On_ConnectionsChanged += checkForNewConnections;

        //do one check at the beginning just in case we're being spawned after startup and after the callbacks
        //have already been called
        if (gyro[0] == null && EasyWiFiController.lastConnectedPlayerNumber >= 0)
        {
            EasyWiFiUtilities.checkForClient(control, (int)player, ref gyro, ref currentNumberControllers);
        }
    }

    void OnDestroy()
    {
        EasyWiFiController.On_ConnectionsChanged -= checkForNewConnections;
    }

    // Update is called once per frame
    void Update()
    {
        //iterate over the current number of connected controllers
        for (int i = 0; i < currentNumberControllers; i++)
        {
            if (gyro[i] != null && gyro[i].serverKey != null && gyro[i].logicalPlayerNumber != EasyWiFiConstants.PLAYERNUMBER_DISCONNECTED)
            {
                mapDataStructureToAction(i);
            }
        }
    }

    public void IncreaseTolerance()
    {
        if (tolerance < maxTolerance)
        {
            tolerance++;
            sensitivity.text = tolerance.ToString();
            UpdateCrosshairPosition();
        }
    }

    public void DecreaseTolerance()
    {
        if (tolerance > minTolerance)
        {
            tolerance--;
            sensitivity.text = tolerance.ToString();
            UpdateCrosshairPosition();
        }
    }

    public void UpdateCrosshairPosition()
    {
        CrossNorth.transform.localPosition = new Vector3(0, 110 + tolerance * scaleCrossHair, 0);
        CrossSouth.transform.localPosition = new Vector3(0, -110 - tolerance * scaleCrossHair, 0);
        CrossWest.transform.localPosition = new Vector3(-110 - tolerance * scaleCrossHair, 0, 0);
        CrossEast.transform.localPosition = new Vector3((110 + tolerance * scaleCrossHair), 0, 0);
    }

    //wifi stuff
    public void mapDataStructureToAction(int index)
    {
        w = gyro[index].GYRO_W;
        x = gyro[index].GYRO_X;
        y = gyro[index].GYRO_Y;
        z = gyro[index].GYRO_Z;

        orientation = new Quaternion(x, y, z, w);
        finalOrientation = Quaternion.Inverse(zeroOrientation) * orientation;
        spiritLevel.rotation = finalOrientation;

        angle_x = Mathf.Abs(spiritLevel.localEulerAngles.x);
        angle_y = Mathf.Abs(spiritLevel.localEulerAngles.y);
        angle_z = Mathf.Abs(spiritLevel.localEulerAngles.z);

        sensitivity.text = tolerance.ToString();

        if (angle_x > tolerance && angle_x < (360 - tolerance))
        {
            Camera.main.backgroundColor = Color.red;
        }
        else if (angle_y > tolerance && angle_y < (360 - tolerance))
        {
            Camera.main.backgroundColor = Color.red;
        }
        else if (angle_z > tolerance && angle_z < (360 - tolerance))
        {
            Camera.main.backgroundColor = Color.red;
        }
        else
            Camera.main.backgroundColor = Color.black;
    }

    public void checkForNewConnections(bool isConnect, int playerNumber)
    {
        EasyWiFiUtilities.checkForClient(control, (int)player, ref gyro, ref currentNumberControllers);
    }


    public void RightHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void LeftHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, -180f, 0);
    }

    public void ChangeAlpha()
    {
        pipCanvas.alpha -= 0.1f;
    }

    override protected IEnumerator TransitionIn()
    {
        OverlayTransitioner.instance.TransitionIn(ScreenType.PIPDisplay);
        InputManager.instance.DisableReticle();
        InputManager.instance.ToggleViewMode();
        InputManager.instance.goDown.AddListener(StartTransitionOut);
        yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
    }

    override protected IEnumerator TransitionOut()
    {
        OverlayTransitioner.instance.TransitionOut();
        InputManager.instance.goDown.RemoveListener(StartTransitionOut);

        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));

        InputManager.instance.ToggleViewMode();
        InputManager.instance.EnableReticle();
        gameObject.SetActive(false);
        CylinderMenu.MenuManager.instance.ExitPIP();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        Material mat1 = spiritLevel.GetComponent<Renderer>().materials[0];
        Material mat2 = PIPPointer.GetComponent<Renderer>().materials[0];
        float t = 0;
        float alpha = startAlpha;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
            mat1.SetFloat("_Alpha", alpha);
            mat2.SetFloat("_Alpha", alpha);
            pipCanvas.alpha = alpha;
            yield return null;
        }
    }
}