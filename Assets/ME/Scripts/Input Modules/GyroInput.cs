using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroInput : InputManager {

	private bool canTrigger = false;

	private Camera cam;
	private Vector3 rotDiff = Vector3.zero;
	//private Vector3 mouseLast = Vector3.zero;

	public float scrollSpeed = 1f;

	private float yaw = 0f;
	private float pitch = 0f;

	private bool rotationMode = true;

	private Quaternion rotStart;

	// Grace period at start of app
	void Start () {
		cam = Camera.main;
		Input.gyro.enabled = true;

		//rotStart = GyroToQuart(Input.gyro.attitude);

		StartCoroutine(PreventMultipleInput());
	}

	Quaternion GyroToQuart(Quaternion q) {
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}


	// Update is called once per frame
	void Update () {


		//cam.transform.rotation = GyroToQuart(Input.gyro.attitude);
		//cam.transform.Rotate(-90f, 0f, 0f);

		//transform.rotation = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
		//transform.rotation = q;
		rotDiff.x = Input.gyro.rotationRateUnbiased.x;
		rotDiff.y = Input.gyro.rotationRateUnbiased.y;


		if (canTrigger) {
			if (rotDiff.y < -moveThreshold.x) {
				Debug.Log("Right");
				goRight.Invoke();
				StartCoroutine(PreventMultipleInput());
			}

			else if (rotDiff.y > moveThreshold.x) {
				Debug.Log("Left");
				goLeft.Invoke();
				StartCoroutine(PreventMultipleInput());
			}

			else if (rotDiff.x > moveThreshold.y) {
				Debug.Log("Up");
				goUp.Invoke();
				StartCoroutine(PreventMultipleInput());
			}

			else if (rotDiff.x < -moveThreshold.y) {
				Debug.Log("Down");
				goDown.Invoke();
				StartCoroutine(PreventMultipleInput());
			}
		}

		if (rotationMode) {
			yaw -= rotDiff.y * scrollSpeed;
			pitch -= rotDiff.x * scrollSpeed;
			cam.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
		}

	}

	IEnumerator PreventMultipleInput () {
		canTrigger = false;
		yield return new WaitForSeconds(0.5f);
		canTrigger = true;
	}

	public override void ToggleViewMode () {
		rotationMode = !rotationMode;
		cam.transform.rotation = Quaternion.identity;
	}

	public override Vector2 Get2DMovement () {
		//return Vector2.ClampMagnitude(mouseDiff, 1.414f) * scrollSpeed;
		return rotDiff * scrollSpeed;
	}
}
