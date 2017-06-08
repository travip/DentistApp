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
			webcam
		}


		public GameObject MenuRowPrefab;

        public MenuRow currentRow;
        public Transform spentMenuContainer;
		public ImageViewer imageViewer;

		public float rowGap = 10f;
        public float moveTime = 0.1f;
        private bool canMove = true;


		private void Awake() {
            if (instance == null || instance == this)
                instance = this;
            else
                Destroy(this);
        }

        // Use this for initialization
        void Start()
        {
            InputManager.instance.goRight.AddListener(MoveMenuRight);
            InputManager.instance.goLeft.AddListener(MoveMenuLeft);
            InputManager.instance.goUp.AddListener(SelectMenuItem);
			InputManager.instance.goDown.AddListener(MoveMenuDown);

			currentRow = transform.Find("First Row").GetComponent<MenuRow>();

			for (int i = 0; i < currentRow.transform.childCount; i++) {
				MenuItem m = currentRow.transform.GetChild(i).GetComponent<MenuItem>();
				if (m != null) {
					currentRow.menuItems.Add(m);
				}
			}

			currentRow.PositionMenuItems();
			/*
			// Generate starting MenuRow
			currentRow = Instantiate(NewMenuRow, transform).GetComponent<MenuRow>();
			currentRow.FillMenuItems(transform . find menu items);
			*/

		}

		public void MoveMenuRight()
        {
            currentRow.MoveRight();
        }

        public void MoveMenuLeft()
        {
            currentRow.MoveLeft();
        }


		public void SelectMenuItem()
		{
			switch (currentRow.selectedItem.selectAction) {
				case MenuManager.SelectAction.subMenu:
					MoveMenuUp();
					break;
				case MenuManager.SelectAction.imageView:
					ImageView();
					break;
				case MenuManager.SelectAction.webcam:
					StartWebcam();
					break;
				default:
					break;
			}
		}

		public void ImageView() {
			// Remove menu navigation listeners
			InputManager.instance.goDown.RemoveListener(MoveMenuDown);
			InputManager.instance.goUp.RemoveListener(SelectMenuItem);
			InputManager.instance.goRight.RemoveListener(MoveMenuRight);
			InputManager.instance.goLeft.RemoveListener(MoveMenuLeft);

			// Add back listener
			InputManager.instance.goDown.AddListener(ExitImageView);

			// Move up to empty space
			Vector3 newPosition = transform.position;
			newPosition.y -= rowGap*1.5f;

			StartCoroutine(SmoothMovement(newPosition));

			// Show the full sized pic
			imageViewer.ViewImage(currentRow.selectedItem.FullSizedPic);
        }

		public void ExitImageView()
		{
			// remove pic container?

			// Remove back listener
			InputManager.instance.goDown.RemoveListener(ExitImageView);

			// Add menu navigation listeners
			InputManager.instance.goRight.AddListener(MoveMenuRight);
			InputManager.instance.goLeft.AddListener(MoveMenuLeft);
			InputManager.instance.goUp.AddListener(SelectMenuItem);
			InputManager.instance.goDown.AddListener(MoveMenuDown);

			// Move back to the row we came from
			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.transform.localPosition.y;

			StartCoroutine(SmoothMovement(newPosition));

			// Hide image
			imageViewer.HideImage();
		}

		public void StartWebcam() {
			Debug.Log("Starting Webcam");
		}

        public void MoveMenuUp()
        {
			if (canMove == false || currentRow.canRotate == false)
				return;

            // Maybe simplify this somehow
			if (currentRow.selectedItem.subMenuItems.Count == 0)
				return;

            MenuRow prevRow = currentRow;

			// Generate new MenuRow and set its list of menu items
			currentRow = Instantiate(MenuRowPrefab, transform).GetComponent<MenuRow>();
			currentRow.transform.position = new Vector3(prevRow.transform.position.x, prevRow.transform.position.y + rowGap, prevRow.transform.position.z);

			currentRow.InitializeMenu(prevRow);

			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.transform.localPosition.y;

			StartCoroutine(SmoothMovement(newPosition));
			StartCoroutine(setCanMove(moveTime * 0.7f));
		}

		public void MoveMenuDown()
		{
			if (canMove == false || currentRow.canRotate == false)
				return;

			if (currentRow.belowRow == null)
				return;

			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.belowRow.transform.localPosition.y;

			MenuRow prevRow = currentRow;
            currentRow = currentRow.belowRow;

			StartCoroutine(BackMenu(newPosition, prevRow));
			StartCoroutine(setCanMove(moveTime * 0.7f));
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

			
			currentRow.canRotate = true;
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
