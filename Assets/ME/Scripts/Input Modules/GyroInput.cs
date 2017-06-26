using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroInput : InputManager
{
	private Vector3 rotDiff = Vector3.zero;

	private float yaw = 0f;
	private float pitch = 0f;

	private Quaternion rotStart;

	// Grace period at start of app
	void Start ()
	{
		Input.gyro.enabled = true;
	}

	// Update is called once per frame
	void Update ()
	{
		rotDiff.x = Input.gyro.rotationRateUnbiased.x;
		rotDiff.y = Input.gyro.rotationRateUnbiased.y;


		if (canTrigger)
		{
			if (rotDiff.y < -moveThreshold.x)
			{
				Debug.Log("Right");
				goRight.Invoke();
			}

			else if (rotDiff.y > moveThreshold.x)
			{
				Debug.Log("Left");
				goLeft.Invoke();
			}

			else if (rotDiff.x > moveThreshold.y)
			{
				Debug.Log("Up");
				goUp.Invoke();
			}

			else if (rotDiff.x < -moveThreshold.y)
			{
				Debug.Log("Down");
				goDown.Invoke();
			}
		}

		if (rotationMode)
		{
			yaw -= rotDiff.y * scrollSpeed;
			pitch -= rotDiff.x * scrollSpeed;
			cam.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
		}
	}

	public override void ResetCamera ()
	{
		base.ResetCamera();
		yaw = 0;
		pitch = 0;
	}

	public override Vector2 Get2DMovement () 
	{
		return rotDiff * scrollSpeed;
	}
}
