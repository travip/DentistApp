﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CylinderMenu
{

    public class MenuManager : TransitionableObject
    {
        public static MenuManager instance { get; private set; }

	    public enum SelectAction
        {
			none,
			subMenu,
		    imageView,
		    webcam,
		    PIP
	    }

		// Object links
		public CanvasGroup title;
		public GameObject MenuRowPrefab;
        public Transform spentMenuContainer;
	    public ImageViewer imageViewer;
		public PIPController pipController;
	    public WebcamViewer webcamViewer;
	    public NetworkManager server;
		public Settings settings;

	    private Camera cam;
		private Text titleText;
		
	    // Runtime variables
	    private bool canMove = true;
	    [HideInInspector]
	    public MenuRow currentRow;
	    private RayCaster raycaster;

	    // Parameters of the menu system
	    [Header("Navigation")]
	    public float turnThresholdMin;
	    public float turnThresholdMax;
	    [Header("Between Rows")]
	    public float rowGap = 10f;
	    public float moveTime = 0.1f;
	    [Header("Rows")]
	    public float circleRadius = 20f;
	    public int maxRows = 1;
	    public int maxColumns = 50;
	    public Vector3 mainMenuItemScale;
	    public float gapBetweenItems = 0.5f;

	    private void Awake()
        {
            if (instance == null || instance == this)
                instance = this;
            else
                Destroy(this);
        }

        // Use this for initialization
        void Start()
        {
		    cam = Camera.main;
			titleText = title.GetComponent<Text>();

			// Generate starting MenuRow
			currentRow = Instantiate(MenuRowPrefab, transform).GetComponent<MenuRow>();
			
		    currentRow.maxRows = 1;
		    currentRow.maxColumns = 5;
		    currentRow.name = "Home Screen";
		    currentRow.startInMiddle = true;

		    for (int i = 0; i < transform.childCount; i++) {
			    MenuItem m = transform.GetChild(i).GetComponent<MenuItem>();
			    if (m != null) {
				    currentRow.menuItems.Add(m);
			    }
		    }

		    currentRow.InitializeMenu(null, circleRadius, maxRows, maxColumns, mainMenuItemScale, gapBetweenItems);

		    currentRow.StartTransitionIn();

			raycaster = RayCaster.instance;
		    raycaster.OnRayEnter += RayEnterHandler;
		    raycaster.OnRayStay += RayStayHandler;
		    raycaster.OnRayExit += RayExitHandler;
		    raycaster.looker = cam.transform;
	    }

	    void Update()
		{
		    // Cast a ray from the middle of the camera into the scene to test if it hit hits any menu selectors
		    raycaster.CastForward();

			
		    float yRot = cam.transform.rotation.eulerAngles.y;
		    if (yRot > 180f) {
			    yRot = yRot - 360f;
		    }
			

		    if (yRot > turnThresholdMin) {
			    float percent = (yRot - turnThresholdMin) / (turnThresholdMax - turnThresholdMin);
			    currentRow.TurnRight(Mathf.Clamp01(percent));
		    } else if (yRot < -turnThresholdMin) {
			    float percent = (-yRot - turnThresholdMin) / (turnThresholdMax - turnThresholdMin);
			    currentRow.TurnLeft(Mathf.Clamp01(percent));
		    }
	    }

	    public void RayEnterHandler(GameObject hit)
        {
		    switch (hit.transform.tag) {
			    case "Selector":
					hit.transform.gameObject.GetComponent<Selector>().LookAt();
					break;
			    case "MenuItem":
				    hit.transform.gameObject.GetComponent<MenuItem>().LookAt();
				    break;
			    default:
				    break;
		    }
	    }

	    public void RayStayHandler (GameObject hit)
        {
		    switch (hit.transform.tag) {
			    case "Selector":
					hit.transform.gameObject.GetComponent<Selector>().LookAt();
				    break;
			    case "MenuItem":
				    //hit.transform.gameObject.GetComponent<MenuItem>().LookAt();
					break;
				default:
				    break;
		    }
	    }

	    public void RayExitHandler(GameObject hit)
        {
		    switch (hit.transform.tag) {
			    case "Selector":
				    //LookAtSelector(hit.transform.gameObject.GetComponent<MenuSelector>());
				    break;
			    case "MenuItem":
				    hit.transform.gameObject.GetComponent<MenuItem>().LookAway();
				    break;
			    default:
				    break;
		    }
	    }

		public void MoveMenuRight()
        {
            currentRow.MoveRight();
        }

        public void MoveMenuLeft()
        {
            currentRow.MoveLeft();
        }

	    public void SelectMenuItem(TransitionItem selected)
	    {
		    currentRow.selectedItem = selected;
		    switch (selected.selectAction) {
			    case MenuManager.SelectAction.subMenu:
				    ToNewRow();
				    break;
			    case MenuManager.SelectAction.imageView:
					LoadModule(0);
				    break;
			    case MenuManager.SelectAction.webcam:
					LoadModule(1);
					break;
			    case MenuManager.SelectAction.PIP:
					LoadModule(2);
					break;
			    default:
				    break;
		    }
	    }

	    public Texture GetNextImage()
        {
		    return currentRow.NextImage().FullSizedPic;
	    }

	    public Texture GetPreviousImage ()
        {
		    return currentRow.PreviousImage().FullSizedPic; ;
	    }

		public void LoadModule (int type) {
			switch (type) {
				case 0:
					imageViewer.LoadImage(currentRow.selectedItem.FullSizedPic);
					StartTransitionOut(imageViewer);
					break;
				case 1:
					server.gameObject.SetActive(true);
					StartTransitionOut(webcamViewer);
					break;
				case 2:
					StartTransitionOut(pipController);
					break;
				default:
					break;
			}
		}

		protected override IEnumerator TransitionOut () {
			currentRow.StartTransitionOut();
			yield return new WaitForSeconds(Constants.Transitions.FadeTime);
			// nothing after
		}

		protected override IEnumerator TransitionIn () {
			currentRow.StartTransitionIn();
			yield return null;
		}

		private IEnumerator FadeBetweenRows (MenuRow before, MenuRow after, bool destroyBefore)
		{
			before.StartTransitionOut();

			yield return StartCoroutine(FadeCanvasGroup(1f, 0f, Constants.Transitions.FadeTime, title));
			titleText.text = after.name;
			StartCoroutine(FadeCanvasGroup(0f, 1f, Constants.Transitions.FadeTime, title));

			if (destroyBefore)
				before.TerminateMenu(spentMenuContainer);

			after.StartTransitionIn();
		}

		public void ToNewRow()
        {
		    if (canMove == false || currentRow.canRotate == false)
			    return;

            // Maybe simplify this somehow
		    if (currentRow.selectedItem.subMenuItems.Count == 0)
			    return;

            MenuRow prevRow = currentRow;

		    // Generate new MenuRow and set its list of menu items
		    currentRow = Instantiate(MenuRowPrefab, transform).GetComponent<MenuRow>();
		    currentRow.transform.position = new Vector3(prevRow.transform.position.x, prevRow.transform.position.y, prevRow.transform.position.z);
		    currentRow.InitializeMenu(prevRow, circleRadius, maxRows, maxColumns, prevRow.selectedItem.rowItemSize, gapBetweenItems);
			currentRow.name = prevRow.selectedItem.itemName;

			StartCoroutine(FadeBetweenRows(prevRow, currentRow, false));
		}

	    public void ToPreviousRow()
	    {
		    if (canMove == false || currentRow.canRotate == false)
			    return;

		    if (currentRow.belowRow == null)
			    return;

		    MenuRow prevRow = currentRow;
            currentRow = currentRow.belowRow;

			StartCoroutine(FadeBetweenRows(prevRow, currentRow, true));
		}

        private IEnumerator SmoothMovement(Vector3 end)
        {
		    Vector3 startPos = transform.position;
		    float t = 0;

            while (t < 1)
            {
			    t += Time.deltaTime / moveTime;
			    t = Mathf.Clamp01(t);

			    transform.position = Vector3.Lerp(startPos, end, Easing.Quadratic.InOut(t));
			    yield return null;
            }
        }

	    private IEnumerator setCanMove (float t)
        {
		    currentRow.canRotate = false;
		    canMove = false;
		    yield return new WaitForSeconds(t);
		    canMove = true;
		    currentRow.canRotate = true;
	    }
    }
}
