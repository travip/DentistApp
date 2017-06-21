using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureVisualiser : MonoBehaviour {

	public Transform horizontal, vertical;

	public Color colorNormal = Color.white;
	public Color colorActivated = Color.red;

	private Vector2 gestureSize;
	private Image horizImage;
	private Image vertImage;

	void Start () {
		horizImage = horizontal.GetComponent<Image>();
		vertImage = vertical.GetComponent<Image>();
		gestureSize = new Vector2();
		SetImageScales();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 mov = InputManager.instance.Get2DMovement();
		gestureSize.y = mov.x / InputManager.instance.moveThreshold.y;
		gestureSize.x = -mov.y / InputManager.instance.moveThreshold.x;

		SetImageScales();
	}

	private void SetImageScales() {
		horizontal.localScale = new Vector2(gestureSize.x, 2f);
		if (gestureSize.x >= 1f || gestureSize.x <= -1f) {
			horizImage.color = colorActivated;
		} else {
			horizImage.color = colorNormal;
		}

		vertical.localScale = new Vector2(2f, gestureSize.y);
		if (gestureSize.y >= 1f || gestureSize.y <= -1f) {
			vertImage.color = colorActivated;
		}
		else {
			vertImage.color = colorNormal;
		}
	}
}
