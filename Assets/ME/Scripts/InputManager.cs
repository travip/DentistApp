using UnityEngine;
using UnityEngine.Events;


public abstract class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

	public Vector2 moveThreshold = new Vector2(5f, 5f);

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

	public abstract void ToggleViewMode ();

    public abstract Vector2 Get2DMovement();
}
