using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : InputManager
{
    private Vector3 rotDiff = Vector3.zero;

	private float yaw = 0f;
	private float pitch = 0f;

	// Update is called once per frame
	void Update ()
	{
		rotDiff.x = Input.GetAxis("Mouse X");
		rotDiff.y = Input.GetAxis("Mouse Y");
		
		if (canTrigger)
        {
            if (rotDiff.x > moveThreshold.x)
            {
                Debug.Log("Right");
                goRight.Invoke();
            }

            else if (rotDiff.x < -moveThreshold.x)
            {
                Debug.Log("Left");
                goLeft.Invoke();
            }

            else if (rotDiff.y > moveThreshold.y)
            {
                Debug.Log("Up");
                goUp.Invoke();
            }

            else if (rotDiff.y < -moveThreshold.y)
            {
                Debug.Log("Down");
                goDown.Invoke();
            }
        }

		if (rotationMode) {
			yaw += rotDiff.x * scrollSpeed;
			pitch -= rotDiff.y * scrollSpeed;
			cam.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
		}

	}

	public override Vector2 Get2DMovement()
    {
		Vector2 mov = rotDiff * scrollSpeed;
		return new Vector2(mov.y, -mov.x);
	}
}
