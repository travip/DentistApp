using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : InputManager {

    private bool canTrigger = false;

	private Camera cam;
    private Vector3 mouseDiff = Vector3.zero;
    //private Vector3 mouseLast = Vector3.zero;

    public float scrollSpeed = 1f;

	private float yaw = 0f;
	private float pitch = 0f;

	private bool mouseRotation = true;

    // Grace period at start of app
    void Start()
    {
		cam = Camera.main;

		StartCoroutine(PreventMultipleInput());
    }

	// Update is called once per frame
	void Update () {

		//mouseDiff = Input.mousePosition - mouseLast;
		//mouseLast = Input.mousePosition;

		mouseDiff.x = Input.GetAxis("Mouse X");
		mouseDiff.y = Input.GetAxis("Mouse Y");
		
		if (canTrigger)
        {
            if (mouseDiff.x > moveThreshold.x)
            {
                Debug.Log("Right");
                goRight.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.x < -moveThreshold.x)
            {
                Debug.Log("Left");
                goLeft.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.y > moveThreshold.y)
            {
                Debug.Log("Up");
                goUp.Invoke();
                StartCoroutine(PreventMultipleInput());
            }

            else if (mouseDiff.y < -moveThreshold.y)
            {
                Debug.Log("Down");
                goDown.Invoke();
                StartCoroutine(PreventMultipleInput());
            }
        }

		if (mouseRotation) {
			yaw += mouseDiff.x * scrollSpeed;
			pitch -= mouseDiff.y * scrollSpeed;
			cam.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
		}

	}

    IEnumerator PreventMultipleInput()
    {
        canTrigger = false;
        yield return new WaitForSeconds(0.5f);
        canTrigger = true;
    }

	public override void ToggleViewMode () {
		mouseRotation = !mouseRotation;
		cam.transform.rotation = Quaternion.identity;
	}

	public override Vector2 Get2DMovement()
    {
		//return Vector2.ClampMagnitude(mouseDiff, 1.414f) * scrollSpeed;
		Vector2 mov = mouseDiff * scrollSpeed;
		
		return new Vector2(mov.y, -mov.x);
	}
}
