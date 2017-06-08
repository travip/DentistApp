using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handle viewing of image
public class ImageViewer : MonoBehaviour
{
	public Vector3 picContainerOffset;

    public Vector3 bottomLeftView;
    public Vector3 topRightView;
    public Vector3 extent;

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
        // Could be called in Awake - unless the camera ever moves or the size of the Quad the image is printed on changes
        bottomLeftView = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 5f));
        topRightView = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 5f));
        extent = GetComponent<Renderer>().bounds.extents;

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

	// Update is called once per frame
	void Update ()
	{
        if (viewingImage)
        {
            Vector2 inputMovement = InputManager.instance.Get2DMovement();
			Vector3 movement = new Vector3(0, inputMovement.y, inputMovement.x);
            Vector3 nextMove = transform.localPosition + movement;

            nextMove = new Vector3(nextMove.x, 
                Mathf.Clamp(nextMove.y, topRightView.y - extent.y, bottomLeftView.y + extent.y),
                Mathf.Clamp(nextMove.z, topRightView.z - extent.z, bottomLeftView.z + extent.z));

            transform.localPosition = nextMove;
        }
	}
}
