using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CylinderMenu;

public class PIPController : TransitionableObject
{
    public static PIPController instance { get; private set; }

    public Transform PIPPointer;
	public Camera orthographicCamera;
	public Camera perspectiveCamera;

    //values and variables
    public Quaternion orientation;
	public Quaternion zeroOrientation = Quaternion.identity;

	public Transform rotator;
    public bool pipSending;

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

    public Renderer pipAlert;
    public RectTransform CrossNorth;
    public RectTransform CrossSouth;
    public RectTransform CrossWest;
    public RectTransform CrossEast;
    public int scaleCrossHair = 2;

    public Text sensitivity;
    public PipTimer timer;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    void OnEnable()
    {
        orientation = Quaternion.identity;
        CrossNorth.localPosition = new Vector3(0, (100 + tolerance * scaleCrossHair), 0);
        CrossSouth.localPosition = new Vector3(0, (-100 - tolerance * scaleCrossHair), 0);
        CrossWest.localPosition = new Vector3((-100 - tolerance * scaleCrossHair), 0, 0);
        CrossEast.localPosition = new Vector3((100 + tolerance * scaleCrossHair), 0, 0);
        sensitivity.text = tolerance.ToString();
        LpmsManager.instance.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(pipSending)
            CalculateOrienatation();
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
        CrossNorth.transform.localPosition = new Vector3(0, 100 + tolerance * scaleCrossHair, 0);
        CrossSouth.transform.localPosition = new Vector3(0, -100 - tolerance * scaleCrossHair, 0);
        CrossWest.transform.localPosition = new Vector3(-100 - tolerance * scaleCrossHair, 0, 0);
        CrossEast.transform.localPosition = new Vector3((100 + tolerance * scaleCrossHair), 0, 0);
    }

	public void ReceiveNetworkedRotation(Quaternion quat) {
		orientation = new Quaternion(0.5f, 0.5f, -0.5f, -0.5f) * quat;
	}

	public void CalculateOrienatation()
    {
        rotator.localRotation = LpmsManager.instance.sensorOrientation;
        float diffAngle = Vector3.Angle(-Vector3.forward, rotator.right);

        if (Mathf.Abs(180-diffAngle) > tolerance)
        {
            pipAlert.materials[0].SetColor("_Tint", Color.red);
        }
        else
            pipAlert.materials[0].SetColor("_Tint", Color.black);
    }

    public void RightHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 90f, 90f);
    }

    public void LeftHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 90f, -90f);
    }

    public void ZeroOrientation()
    {
        LpmsManager.instance.ResetOrientationObject();
        timer.ResetTimer();
    }

    public void Back()
	{
        StartTransitionOut(CylinderMenu.MenuManager.instance);
	}

	private void SwitchToOrthographicCamera()
	{
		perspectiveCamera.enabled = false;
		perspectiveCamera.gameObject.SetActive(false);
		orthographicCamera.enabled = true;
		orthographicCamera.gameObject.SetActive(true);
	}

	private void SwitchToPerspectiveCamera()
	{
		perspectiveCamera.enabled = true;
		perspectiveCamera.gameObject.SetActive(true);
		orthographicCamera.enabled = false;
		orthographicCamera.gameObject.SetActive(false);
	}

    override protected IEnumerator TransitionIn()
    {
		SwitchToOrthographicCamera();

        PipTimer.instance.LoadTimer();
		InputManager.instance.ToggleViewMode();
        InputManager.instance.goDown.AddListener(Back);
        InputManager.instance.goLeft.AddListener(ZeroOrientation);
        InputManager.instance.goRight.AddListener(ZeroOrientation);
        yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
    }

    override protected IEnumerator TransitionOut()
    {
        InputManager.instance.goDown.RemoveListener(Back);
        InputManager.instance.goLeft.RemoveListener(ZeroOrientation);
        InputManager.instance.goRight.RemoveListener(ZeroOrientation);
        PipTimer.instance.UnloadTimer();

        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));

        pipAlert.materials[0].color = Color.black;
        InputManager.instance.ToggleViewMode();

		SwitchToPerspectiveCamera();
	}

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        Material mat1 = rotator.GetComponent<Renderer>().materials[0];
        Material mat2 = PIPPointer.GetComponent<Renderer>().materials[0];
        Material mat3 = pipAlert.materials[0];

		Color col1 = mat1.color;
		Color col2 = mat2.color;
        float t = 0;
        float alpha = startAlpha;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
			col1.a = alpha;
			col2.a = alpha;
			mat1.color = col1;
			mat2.color = col2;
            mat3.SetFloat("_Alpha", alpha);
            yield return null;
        }
    }
}
