using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public abstract class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

	public Vector2 moveThreshold = new Vector2(5f, 5f);

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
            instance = this;
        else
            Destroy(this);
    }

    public void ToggleViewModeDelayed(float seconds)
    {
        Invoke("ToggleViewMode", seconds);
    }

    public void EnableReticle()
    {
        reticle.SetActive(true);
    }

    public void DisableReticle()
    {
        reticle.SetActive(false);
    }
	public abstract void ToggleViewMode ();

    public abstract Vector2 Get2DMovement();
}
