using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

	private iSelectable selection;

	public bool selectOnce = true;

	public float totalTime = 0.6f;

	public Transform scalingImage;
	public float minScale = 0f;
	public float maxScale = 3.1f;

	public float cooloffSpeed = 1f;

	public float selectedZ;
	public float selectMoveSpeed;

	private float currentTime = 0f;
	private Transform imagesTransform;

	private bool hitThisFrame;
	private float targetZ;
	private SelectionSpinner spinner;

	void Start () {
		selection = GetComponent<iSelectable>();
		imagesTransform = transform.Find("Images");
		targetZ = 0f;
		selectedZ = -selectedZ;
		spinner = GetComponent<SelectionSpinner>();
	}

	public void LookAt () {
		targetZ = selectedZ;
		if (spinner != null)
			spinner.selecting = true;

		hitThisFrame = true;
		currentTime += Time.deltaTime;
		SetInsideSize(currentTime / totalTime);

		if (currentTime > totalTime) {
			currentTime = 0f;
			NoSelection();
			selection.Select();
		}
	}

	private void SetInsideSize (float percent) {
		float newScale = minScale + (percent * (maxScale - minScale));
		scalingImage.localScale = new Vector3(newScale, newScale, newScale);
	}

	void Update () {
		if (currentTime > 0f && hitThisFrame == false) {
			currentTime -= Time.deltaTime * cooloffSpeed;
			NoSelection();
		}

		if (imagesTransform.localPosition.z != targetZ) {
			MoveTowardsTarget();
		}

		hitThisFrame = false;
	}

	void NoSelection () {
		SetInsideSize(currentTime / totalTime);
		targetZ = 0f;
		if (spinner != null)
			spinner.selecting = false;
	}

	void MoveTowardsTarget () {
		if (imagesTransform.localPosition.z > targetZ) {
			Vector3 v = imagesTransform.localPosition;
			v.z -= Time.deltaTime * selectMoveSpeed;
			if (v.z < targetZ)
				v.z = targetZ;

			imagesTransform.localPosition = v;
		}
		else {
			Vector3 v = imagesTransform.localPosition;
			v.z += Time.deltaTime * selectMoveSpeed;
			if (v.z > targetZ)
				v.z = targetZ;

			imagesTransform.localPosition = v;
		}
	}
}

public interface iSelectable {
	void Select ();
}
