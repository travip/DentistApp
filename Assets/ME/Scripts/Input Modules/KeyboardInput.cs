using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardInput : InputManager
{
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			goRight.Invoke();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			goLeft.Invoke();
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			goUp.Invoke();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			goDown.Invoke();
		}
	}

	public override Vector2 Get2DMovement()
	{
		float horz = Input.GetAxis("Horizontal");
		float vert = Input.GetAxis("Vertical");
		return new Vector2(horz, vert) * scrollSpeed;
	}
}
