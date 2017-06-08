using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
public class ImageViewer : MonoBehaviour {

    public Texture2D image;

    public bool viewingImage;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        if (viewingImage)
        {
            Vector2 movement = InputManager.instance.Get2DMovement();
            transform.Translate(new Vector3(movement.x/100, movement.y/100, 0));
        }
	}
}
