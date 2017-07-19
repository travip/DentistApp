using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public abstract class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

	public Vector2 moveThreshold = new Vector2(2f, 2f);
	public float scrollSpeed = 2f;
	public float preventMovementTime = 0.3f;

	protected bool rotationMode = true;
	protected bool canTrigger = false;
	protected Camera cam;

	[SerializeField]
    private GameObject reticle;

	[HideInInspector]
	public UnityEvent goRight;
	[HideInInspector]
	public UnityEvent goLeft;
	[HideInInspector]
	public UnityEvent goUp;
	[HideInInspector]
	public UnityEvent goDown;

    private void Awake()
    {
		if (instance == null || instance == this)
		{
			instance = this;
			Initialise();
		}
		else
			Destroy(this);
    }

	private void Initialise()
	{
		cam = Camera.main;
		goLeft.AddListener(PreventMultipleInput);
		goRight.AddListener(PreventMultipleInput);
		goUp.AddListener(PreventMultipleInput);
		goDown.AddListener(PreventMultipleInput);

		PreventMultipleInput();
	}

    public void ToggleViewModeDelayed(float seconds)
    {
        Invoke("ToggleViewMode", seconds);
    }

	virtual public void ToggleViewMode ()
	{
		reticle.SetActive(!reticle.activeSelf);
		rotationMode = !rotationMode;
		ResetCamera();
	}

	virtual public void ResetCamera() {
		cam.transform.rotation = Quaternion.identity;
		StartCoroutine(PreventMovement(preventMovementTime));
	}

	private void PreventMultipleInput()
	{
		StartCoroutine(PreventMultipleInputRoutine());
	}

	private IEnumerator PreventMultipleInputRoutine ()
	{
		canTrigger = false;
		yield return new WaitForSeconds(0.15f);
		canTrigger = true;
	}

	private IEnumerator PreventMovement(float time)
	{
		enabled = false;
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			yield return null;
		}
		enabled = true;
	}

	public abstract Vector2 Get2DMovement();
}
