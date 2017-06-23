using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSpinner : MonoBehaviour {

	public Transform Inside, Middle, Outside;

	[Header("Normal")]
	public float rotateSpeedInside;
	public float rotateSpeedMiddle;
	public float rotateSpeedOutside;
	[Header("Selecting")]
	public float rotateFastInside;
	public float rotateFastMiddle;
	public float rotateFastOutside;
	// Update is called once per frame
	void Update () {
		Inside.Rotate(0f, 0f, rotateSpeedInside * Time.deltaTime);
		Middle.Rotate(0f, 0f, rotateSpeedMiddle * Time.deltaTime);
		Outside.Rotate(0f, 0f, rotateSpeedOutside * Time.deltaTime);
	}
}
