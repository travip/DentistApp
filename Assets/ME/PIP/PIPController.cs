using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PIPController : TransitionableObject
{
    public static PIPController instance { get; private set; }

    public Transform PIPPointer;

    //values and variables
    public Quaternion orientation;
    Quaternion zeroOrientation = Quaternion.identity;
    Quaternion finalOrientation;

    public Transform spiritLevel;
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
        orientation = Quaternion.identity;
        CrossNorth.localPosition = new Vector3(0, (110 + tolerance * scaleCrossHair), 0);
        CrossSouth.localPosition = new Vector3(0, (-110 - tolerance * scaleCrossHair), 0);
        CrossWest.localPosition = new Vector3((-110 - tolerance * scaleCrossHair), 0, 0);
        CrossEast.localPosition = new Vector3((110 + tolerance * scaleCrossHair), 0, 0);
        sensitivity.text = tolerance.ToString();
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
        CrossNorth.transform.localPosition = new Vector3(0, 110 + tolerance * scaleCrossHair, 0);
        CrossSouth.transform.localPosition = new Vector3(0, -110 - tolerance * scaleCrossHair, 0);
        CrossWest.transform.localPosition = new Vector3(-110 - tolerance * scaleCrossHair, 0, 0);
        CrossEast.transform.localPosition = new Vector3((110 + tolerance * scaleCrossHair), 0, 0);
    }

	public void ReceiveNetworkedRotation(Quaternion quat) {
		orientation = ReadGyroscopeRotation(quat);
	}


	private Quaternion ReadGyroscopeRotation (Quaternion quat) {
		return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * quat * new Quaternion(0, 0, 1, 0);
	}

	//wifi stuff
	public void CalculateOrienatation()
    {
		//finalOrientation = Quaternion.Inverse(zeroOrientation) * orientation;
		//orientation = ReadGyroscopeRotation(Input.gyro.attitude);
		finalOrientation = orientation * Quaternion.Inverse(zeroOrientation);
		spiritLevel.rotation = finalOrientation;

        angle_x = Mathf.Abs(spiritLevel.localEulerAngles.x);
        angle_y = Mathf.Abs(spiritLevel.localEulerAngles.y);
        angle_z = Mathf.Abs(spiritLevel.localEulerAngles.z);

        sensitivity.text = tolerance.ToString();

        if (angle_x > tolerance && angle_x < (360 - tolerance))
        {
            pipAlert.materials[0].SetColor("_Tint", Color.red);
        }
        else if (angle_y > tolerance && angle_y < (360 - tolerance))
        {
            pipAlert.materials[0].SetColor("_Tint", Color.red);
        }
        else if (angle_z > tolerance && angle_z < (360 - tolerance))
        {
            pipAlert.materials[0].SetColor("_Tint", Color.red);
        }
        else
            pipAlert.materials[0].SetColor("_Tint", Color.black);
    }

    public void RightHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void LeftHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, -180f, 0);
    }

    public void ZeroOrientation()
    {
		if (!pipSending)
		{
			NetworkManager.instance.StartPIPDataStream();
		}
		zeroOrientation = orientation;
    }

	public void Back() {
		StartTransitionOut(CylinderMenu.MenuManager.instance);
	}

    override protected IEnumerator TransitionIn()
    {
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

        NetworkManager.instance.StopPIPDataStream();

        yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));

        pipAlert.materials[0].color = Color.black;
        InputManager.instance.ToggleViewMode();
	}

    private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
    {
        Material mat1 = spiritLevel.GetComponent<Renderer>().materials[0];
        Material mat2 = PIPPointer.GetComponent<Renderer>().materials[0];
        Material mat3 = pipAlert.materials[0];
        float t = 0;
        float alpha = startAlpha;

        while (t < totalTime)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
            mat1.SetFloat("_Alpha", alpha);
            mat2.SetFloat("_Alpha", alpha);
            mat3.SetFloat("_Alpha", alpha);
            yield return null;
        }
    }
}
