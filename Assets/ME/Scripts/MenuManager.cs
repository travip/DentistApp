using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuManager : MonoBehaviour
    {

        public static MenuManager instance { get; private set; }

		public GameObject MenuRowPrefab;

        public MenuRow currentRow;
        public Transform spentMenuContainer;

		public float rowGap = 10f;
        public float moveTime = 0.1f;
        private bool isMoving = false;

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
            InputManager.instance.goUp.AddListener(MoveMenuUp);
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

        public void MoveMenuUp()
        {
			if (isMoving)
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
        }

		public void MoveMenuDown()
		{
			if (isMoving)
				return;

			if (currentRow.belowRow == null)
				return;

			Vector3 newPosition = transform.position;
			newPosition.y = -currentRow.belowRow.transform.localPosition.y;

			MenuRow prevRow = currentRow;
            currentRow = currentRow.belowRow;
			StartCoroutine(BackMenu(newPosition, prevRow));
		}

        
		private IEnumerator BackMenu(Vector3 end, MenuRow prevRow) {
			yield return StartCoroutine(SmoothMovement(end));
			prevRow.TerminateMenu(spentMenuContainer);       
		}
        

        private IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;

			Vector3 startPos = transform.position;
			float t = 0;

            while (t < 1)
            {
				t += Time.deltaTime / moveTime;
				t = Mathf.Clamp01(t);

				transform.position = Vector3.Lerp(startPos, end, Easing.Quadratic.InOut(t));
				yield return null;
            }

            isMoving = false;
        }
    }
}
