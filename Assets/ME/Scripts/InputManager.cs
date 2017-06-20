using UnityEngine;
using UnityEngine.Events;


public abstract class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

	public float moveThreshold = 5f;

	public UnityEvent goRight;
    public UnityEvent goLeft;
    public UnityEvent goUp;
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
