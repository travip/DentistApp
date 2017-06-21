using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Handedness : MonoBehaviour {

	Quaternion pointerPosition;
	public float rightY=0;
	public float leftY=-180;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
	}
		public void LeftHanded (){
		transform.localRotation=Quaternion.Euler (0, leftY, 0);

		}

		public void RightHanded (){
		transform.localRotation=Quaternion.Euler (0, rightY, 0);

	}
	}

