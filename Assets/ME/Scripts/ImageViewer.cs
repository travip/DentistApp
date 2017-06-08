using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
public class ImageViewer : MonoBehaviour
{
	public Vector3 picContainerOffset;

	private bool viewingImage;
	private Material mat;

	// Use this for initialization
	void Awake ()
	{
		mat = GetComponent<Renderer>().material;
		gameObject.SetActive(false);
	}

	public void ViewImage(Texture image)
	{
		mat.mainTexture = image;
		viewingImage = true;

		transform.position = picContainerOffset; 
		gameObject.SetActive(true);
	}

	public void HideImage()
	{
		mat.mainTexture = null;
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update ()
	{
        if (viewingImage)
        {
            Vector2 inputMovement = InputManager.instance.Get2DMovement();
			Vector3 movement = new Vector3(0f, -inputMovement.x, inputMovement.y);
            transform.Translate(movement);
        }
	}
}
