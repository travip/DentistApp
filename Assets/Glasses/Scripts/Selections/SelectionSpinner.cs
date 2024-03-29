﻿using System.Collections;
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

	[HideInInspector]
	public bool selecting = false;

	// Update is called once per frame
	void Update () {
		if (selecting)
		{
			Inside.Rotate(0f, 0f, rotateFastInside * Time.deltaTime);
			Middle.Rotate(0f, 0f, rotateFastMiddle * Time.deltaTime);
			Outside.Rotate(0f, 0f, rotateFastOutside * Time.deltaTime);
		} else
		{
			Inside.Rotate(0f, 0f, rotateSpeedInside * Time.deltaTime);
			Middle.Rotate(0f, 0f, rotateSpeedMiddle * Time.deltaTime);
			Outside.Rotate(0f, 0f, rotateSpeedOutside * Time.deltaTime);
		}
	}
}
