using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance { get; private set; }

		public enum SelectAction {
			subMenu,
			imageView,
			webcam,
			PIP
		}

		// Object links
		public GameObject MenuRowPrefab;
        public Transform spentMenuContainer;
		public ImageViewer imageViewer;
		public WebcamViewer webcamViewer;
		public NetworkManager server;
        public OverlayTransitioner overlayTransitioner;

		private Camera cam;
		

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
		public Vector3 itemScale;
		public float gapBetweenItems = 0.5f;




		private void Awake() {
            if (instance == null || instance == this)
                instance = this;
            else
                Destroy(this);
        }

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
			prevR = circleRadius;
			prevRows = maxRows;
			prevCols = maxColumns;
			prevScale = itemScale;
			prevGap = gapBetweenItems;
#endif
			cam = Camera.main;

			//InputManager.instance.goRight.AddListener(MoveMenuRight);
			//InputManager.instance.goLeft.AddListener(MoveMenuLeft);
			//InputManager.instance.goUp.AddListener(SelectMenuItem);
			//InputManager.instance.goDown.AddListener(MoveMenuDown);

			// Generate starting MenuRow
			currentRow = Instantiate(MenuRowPrefab, transform).GetComponent<MenuRow>();
			
			currentRow.maxRows = 1;
			currentRow.maxColumns = 5;
			currentRow.name = "Main Row";
			currentRow.startInMiddle = true;

			for (int i = 0; i < transform.childCount; i++) {
				MenuItem m = transform.GetChild(i).GetComponent<MenuItem>();
				if (m != null) {
					currentRow.menuItems.Add(m);
				}
			}

			currentRow.InitializeMenu(null, circleRadius, maxRows, maxColumns, mainMenuItemScale, gapBetweenItems);
			currentRow.TransitionIn();

			raycaster = new RayCaster();
			raycaster.OnRayEnter += RayEnterHandler;
			raycaster.OnRayStay += RayStayHandler;
			raycaster.OnRayExit += RayExitHandler;
			raycaster.looker = cam.transform;
		}

		void Update() {
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
			
			//////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////      DEBUG STUFF      ///////////////////////////////////////////////
			//////////////////////////////// REMOVE FOR PRODUCTION ///////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////

			#if UNITY_EDITOR

				if (prevR != circleRadius || prevRows != maxRows || prevCols != maxColumns || prevScale != itemScale || prevGap != gapBetweenItems) {
					// recalculate row
					currentRow.RecalculateRow(circleRadius, maxRows, maxColumns, itemScale, gapBetweenItems);
				}
				prevR = circleRadius;
				prevRows = maxRows;
				prevCols = maxColumns;
				prevScale = itemScale;
				prevGap = gapBetweenItems;

			#endif

		}

		#if UNITY_EDITOR
			private float prevR=0, prevRows=0, prevCols=0, prevGap=0;
			private Vector3 prevScale = Vector3.zero;
		#endif
		////////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////// END DEBUG STUFF ///////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////

		public void RayEnterHandler(GameObject hit) {
			switch (hit.transform.tag) {
				case "Selector":
					LookAtSelector(hit.transform.gameObject.GetComponent<MenuSelector>());
					break;
				case "MenuItem":
					hit.transform.gameObject.GetComponent<MenuItem>().LookAt();
					break;
				default:
					break;
			}
		}

		public void RayStayHandler (GameObject hit) {
			switch (hit.transform.tag) {
				case "Selector":
					LookAtSelector(hit.transform.gameObject.GetComponent<MenuSelector>());
					break;
				case "MenuItem":
					//hit.transform.gameObject.GetComponent<MenuItem>().LookAt();
					break;
				default:
					break;
			}
		}

		public void RayExitHandler(GameObject hit) {
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
		

		private void LookAtSelector(MenuSelector menuHit)
		{
			
			// If hitting a menu selector, check with the selector if it's pressed yet
			if (menuHit.LookAt()) {
				switch (menuHit.selectionType) {
					case MenuSelector.SelectionType.select:
						SelectMenuItem(menuHit.parentMenuItem);
						break;
					case MenuSelector.SelectionType.back:
						ToPreviousRow();
						break;
					case MenuSelector.SelectionType.nextPage:
						MoveMenuRight();
						break;
					case MenuSelector.SelectionType.previousPage:
						MoveMenuLeft();
						break;
					default:
						break;
				}
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

		public void SelectMenuItem(MenuItem selected)
		{
			currentRow.selectedItem = selected;
			switch (selected.selectAction) {
				case MenuManager.SelectAction.subMenu:
					ToNewRow();
					break;
				case MenuManager.SelectAction.imageView:
					ImageView();
					break;
				case MenuManager.SelectAction.webcam:
					StartWebcam();
					break;
				case MenuManager.SelectAction.PIP:
					StartPIP();
					break;
				default:
					break;
			}
		}

		public void ImageView()
        {
			// Move up to empty space
			Vector3 newPosition = transform.position;
			newPosition.y -= rowGap*1.5f;

			StartCoroutine(SmoothMovement(newPosition));

			// Show the full sized pic
			imageViewer.ViewImage(currentRow.selectedItem.FullSizedPic);
        }

		public Texture ImageViewerNext()
        {
			return currentRow.NextImage().FullSizedPic;
		}

		public Texture ImageViewerPrevious ()
        {
			return currentRow.PreviousImage().FullSizedPic; ;
		}

		public void ExitImageView()
		{
			// Move back to the row we came from
			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.transform.localPosition.y;

			StartCoroutine(SmoothMovement(newPosition));
		}

		public void StartWebcam()
        {
			Debug.Log("Starting Webcam");

			// Move up to empty space
			Vector3 newPosition = transform.position;
			newPosition.y -= rowGap * 1.5f;

			StartCoroutine(SmoothMovement(newPosition));

			// Start the server and the webcam viewer
			server.gameObject.SetActive(true);
			webcamViewer.ViewWebcam(server.myImage);
		}

		public void StartPIP()
        {
            overlayTransitioner.TransitionTo(ScreenType.PIPDisplay, true);
            currentRow.TransitionOut(null);
            Debug.Log("Starting PIP");
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
			//currentRow.transform.position = new Vector3(prevRow.transform.position.x, prevRow.transform.position.y + rowGap, prevRow.transform.position.z);
			currentRow.transform.position = new Vector3(prevRow.transform.position.x, prevRow.transform.position.y, prevRow.transform.position.z);

			currentRow.InitializeMenu(prevRow, circleRadius, maxRows, maxColumns, itemScale, gapBetweenItems);

			// Transition to new row
			/*
			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.transform.localPosition.y;

			StartCoroutine(SmoothMovement(newPosition));
			StartCoroutine(setCanMove(moveTime * 0.7f));
			*/

			prevRow.TransitionOut(currentRow);
		}

		public void ToPreviousRow()
		{
			if (canMove == false || currentRow.canRotate == false)
				return;

			if (currentRow.belowRow == null)
				return;

			MenuRow prevRow = currentRow;
            currentRow = currentRow.belowRow;
			
			prevRow.TransitionOut(currentRow, spentMenuContainer);
			

			// Transition to previous row
			/*
			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.belowRow.transform.localPosition.y;
			StartCoroutine(BackMenu(newPosition, prevRow));
			StartCoroutine(setCanMove(moveTime * 0.7f));
			*/
		}


		private IEnumerator BackMenu(Vector3 end, MenuRow prevRow) {
			yield return StartCoroutine(SmoothMovement(end));
			prevRow.TerminateMenu(spentMenuContainer);       
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

		private IEnumerator setCanMove (float t) {
			currentRow.canRotate = false;
			canMove = false;
			yield return new WaitForSeconds(t);
			canMove = true;
			currentRow.canRotate = true;
		}
	}
}
