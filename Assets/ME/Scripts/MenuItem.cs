using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuItem : TransitionableObject {

	    // Object Links
	    public GameObject PicPrefab;
	    public GameObject SelectorPrefab;
	    // Get an explicit reference to the submenu transform so we can use it in dynamic menu creation
	    [HideInInspector]
	    public List<MenuItem> subMenuItems;
	    private Collider col;

	    [Space(10)]

	    // Parameters
	    public float hoverForward;
	    public float hoverTime = 2f;
        public string itemName;
	    public Texture menuPic;
	    public MenuManager.SelectAction selectAction;

	    private Transform pic;
	    private Transform selector;

	    [HideInInspector]
	    public Texture FullSizedPic;

	    // Runtime variables
	    private float zDistance;
	    private float zDistanceHovering;

        // Initialize List
        private void Awake()
        {
		    col = GetComponent<Collider>();

		    subMenuItems = new List<MenuItem>();

		    // Get each MenuItem from the SubMenu container
		    foreach (Transform sub in transform) {
			    subMenuItems.Add(sub.GetComponent<MenuItem>());
		    }

		    // Instantiate the mesh that holds the menu picture
		    pic = Instantiate(PicPrefab, transform).transform;
		    pic.Find("mesh")/*.transform.Find("Cylinder")*/.GetComponent<Renderer>().material.mainTexture = menuPic;

		    // Instantiate selector
		    selector = Instantiate(SelectorPrefab, transform).transform;

		    selector.gameObject.SetActive(false);
		    gameObject.SetActive(false);
	    }

	    public void AddToMenuRow(Transform row, float distance, Quaternion rotation, Vector3 scale)
	    {
		    transform.SetParent(row);
		    transform.localPosition = Vector3.zero;
		    transform.localRotation = rotation;

		    BoxCollider col = GetComponent<BoxCollider>();
		    col.center = new Vector3(0f, 0f, distance);
		    col.size = new Vector3(scale.x, scale.y, 0.1f);

		    zDistance = distance;
		    zDistanceHovering = distance - hoverForward;

		    pic.localPosition = new Vector3(0f, 0f, zDistance);
		    pic.localScale = new Vector3(scale.x, scale.y, scale.z);

		    // slight magic number for the y position of the selector
		    selector.localPosition = new Vector3(0f, scale.y * -0.14f, distance);

		    gameObject.SetActive(true);
	    }

	    public void LookAt()
	    {
		    StartCoroutine(SmoothMovement(new Vector3(0f, 0f, zDistanceHovering)));
		    selector.gameObject.SetActive(true);
	    }

	    public void LookAway()
	    {
		    StartCoroutine(SmoothMovement(new Vector3(0f, 0f, zDistance)));
		    selector.gameObject.SetActive(false);
	    }

	    private IEnumerator SmoothMovement (Vector3 end)
	    {
		    Vector3 startPos = pic.transform.localPosition;
		    float t = 0;

		    while (t < 1)
		    {
			    t += Time.deltaTime / hoverTime;
			    t = Mathf.Clamp01(t);

			    Vector3 newPos = Vector3.Lerp(startPos, end, t);
			    pic.transform.localPosition = newPos;
			    selector.localPosition = new Vector3(0f, selector.localPosition.y, newPos.z);
			    yield return null;
		    }
	    }


		// Transitions

		override protected IEnumerator TransitionOut () {
			col.enabled = false;
			yield return StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));

			gameObject.SetActive(false);
		}

		override protected IEnumerator TransitionIn () {
			col.enabled = true;
			yield return StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
		}

		private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
        {
		    Material mat1 = pic.Find("mesh").GetComponent<Renderer>().materials[0];

		    float t = 0;
		    float alpha = startAlpha;

		    while (t < totalTime) {
			    t += Time.deltaTime;
			    alpha = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
			    mat1.SetFloat("_Alpha", alpha);
			    yield return null;
		    }
	    }
    }

}
