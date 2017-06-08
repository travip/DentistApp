using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : InputManager {

    private bool canTrigger = false;

    public float mouseMoveThreshold = 50;
    private Vector3 mouseDiff = Vector3.zero;
    private Vector3 mouseLast = Vector3.zero;

    public float scrollSpeed = 1;

    // Grace period at start of app
    void Start()
    {
        StartCoroutine(PreventMultipleInput());
    }

	// Update is called once per frame
	void Update () {

        mouseDiff = Input.mousePosition - mouseLast;
        mouseLast = Input.mousePosition;

        if (canTrigger)
        {
            if (mouseDiff.x > mouseMoveThreshold)
            {
                Debug.Log("Right");
                goRight.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.x < -mouseMoveThreshold)
            {
                Debug.Log("Left");
                goLeft.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.y > mouseMoveThreshold)
            {
                Debug.Log("Up");
                goUp.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.y < -mouseMoveThreshold)
            {
                Debug.Log("Down");
                goDown.Invoke();
                StartCoroutine(PreventMultipleInput());
            }
        }
    }

    IEnumerator PreventMultipleInput()
    {
        canTrigger = false;
        yield return new WaitForSeconds(0.5f);
        canTrigger = true;
    }

    public override Vector2 Get2DMovement()
    {
        return Vector2.ClampMagnitude(mouseDiff, 1.414f) * scrollSpeed;
    }
}
