using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
public class ImageViewer : MonoBehaviour
{
	public Vector3 picContainerOffset;

    private Vector3 bottomLeftView;
	private Vector3 topRightView;
	private Vector3 extent;
	private Vector2 clampHigh;
	private Vector2 clampLow;

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

		// Set up image clamp so the user cannot scroll beyond the boundary of the image
        // Could be called in Awake - unless the camera ever moves or the size of the Quad the image is printed on changes
        bottomLeftView = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 5f));
        topRightView = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 5f));
        extent = GetComponent<Renderer>().bounds.extents;
		clampHigh = new Vector2(topRightView.x - extent.x, topRightView.y - extent.y);
		clampLow = new Vector2(bottomLeftView.x + extent.x, bottomLeftView.y + extent.y);


		transform.position = picContainerOffset; 
		gameObject.SetActive(true);
        StartCoroutine(InputGracePeriod());
    }

	public void HideImage()
	{
		mat.mainTexture = null;
		gameObject.SetActive(false);
	}

    private IEnumerator InputGracePeriod()
    {
        yield return new WaitForSeconds(0.5f);
        viewingImage = true;
    }

	void Update ()
	{
        if (viewingImage)
        {
			// Scroll around the image when the looking around
            Vector3 inputMovement = InputManager.instance.Get2DMovement();
            Vector3 nextMove = transform.position + inputMovement;

			nextMove.x = Mathf.Clamp(nextMove.x, clampHigh.x, clampLow.x);
			nextMove.y = Mathf.Clamp(nextMove.y, clampHigh.y, clampLow.y);

			transform.position = nextMove;
        }
	}
}
