using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuRow : MonoBehaviour
    {
		private float turnTick = 10f;
		public float picSpacingX = 12f;
		public float picSpacingY = 12f;

		public GameObject BackSelectorPrefab, NextPageSelectorPrefab, PreviousPageSelectorPrefab;
		public List<MenuItem> menuItems;
        //private int selectedIndex = 0;

        public MenuRow belowRow;

        public float rotateTime = 0.1f;
        public bool canRotate = true;

		public int maxRows = 2;
		public int maxColumns = 5;

		private float radius;
		private float imageSize;

		IEnumerator movement;

		private int pages = 1;
		//private int currentPage = 0;

		private Transform navButtons;
		
		//public MenuItem selectedItem
		//{
		//	get { return menuItems[selectedIndex]; }
		//}
		

		[HideInInspector]
		public MenuItem selectedItem;

        // Dynamic creation of menu
        public void InitializeMenu(MenuRow parentRow, float _radius, float _imageSize)
        {
			radius = _radius;
			imageSize = _imageSize;
			transform.Find("mesh").localScale = new Vector3(radius+1f, radius+1f, 3f);

			gameObject.SetActive(true);

			if (parentRow != null)
			{
				belowRow = parentRow;
				menuItems = belowRow.selectedItem.subMenuItems;
				PositionMenuItems();
				CreateMainButtons();
			} else
			{
				PositionMenuItems();
			}
			
        }

		public void PositionMenuItems() {
			// Need to track index for proper positioning

			// Calculate the number of columns required
			int numRows = maxRows;
			int numColumns = Mathf.CeilToInt((float)menuItems.Count / (float)numRows);

			pages = Mathf.CeilToInt((float)menuItems.Count / (float)(maxColumns * maxRows));

			if (pages == 0)
				pages = 1;
			
			if (numColumns > maxColumns) {
				numColumns = maxColumns;
				// Only display a certain amount of columns
			}


			// The columns are centered based on how many columns there are. 
			// The first column's rotation therefore depends on how many columns there are
			float startRotY = ((numColumns / 2f) - 0.5f) * -picSpacingX;
			float startRotX = ((numRows / 2f) - 0.5f) * -picSpacingY;

			int col = 0, row = 0;
			for (int i = 0; i < menuItems.Count; i++) {
				// Some of this might be able to be done in MenuItem or set beforehand

				menuItems[i].AddToMenuRow(transform, radius, Quaternion.Euler(new Vector3(startRotX + picSpacingY * row, startRotY + picSpacingX * col, 0f)), imageSize);

				row++;
				
				if (row >= numRows)
				{
					row = 0;
					col++;
					if (col >= numColumns)
					{
						break;
					}
				}
					
				
			}
		}

		private void CreateMainButtons() {
			// Create back button and navigation buttons (left/right)
			navButtons = new GameObject().transform;
			navButtons.parent = MenuManager.instance.transform;
			navButtons.transform.position = new Vector3(0f, transform.position.y - 2f, radius);

			Instantiate(BackSelectorPrefab, navButtons);
			if (pages > 1)
			{
				Instantiate(NextPageSelectorPrefab, navButtons).transform.localPosition = new Vector3(2f, 0f, 0f);
				Instantiate(PreviousPageSelectorPrefab, navButtons).transform.localPosition = new Vector3(-2f, 0f, 0f);
			}
			
		}

        // Remove menu items - put them in the faraway land where they wont get in our way
        public void TerminateMenu(Transform container)
        {
            foreach(MenuItem item in menuItems)
            {
                item.transform.SetParent(container);
            }

			menuItems = null;

			Destroy(navButtons.gameObject);
			Destroy(gameObject);
        }

        public void MoveRight()
        {
			if (!canRotate)
				return;

			//if (selectedIndex < menuItems.Count - 1)
			//{
			//    selectedIndex++;
			//	MoveToSelectedItem();
			//}
			RotateView(-turnTick);
        }

        public void MoveLeft()
        {
			if (!canRotate)
				return;

			//if (selectedIndex > 0)
            //{
            //    selectedIndex--;
			//	MoveToSelectedItem();
			//}

			RotateView(turnTick);
		}

		private void RotateView(float degrees) {
			if (movement != null)
				StopCoroutine(movement);

			//Quaternion quart = Quaternion.Euler(0f, -turnTick * (selectedIndex), 0f);
			Quaternion quart = transform.rotation * Quaternion.Euler(Vector3.up * degrees);
			
			movement = SmoothRotation(quart);
			StartCoroutine(movement);
			StartCoroutine(setCanRotate(rotateTime * 0.6f));
		}

		/*private void MoveToSelectedItem() {

			if (movement != null)
				StopCoroutine(movement);

			Quaternion quart = Quaternion.Euler(0f, -turnTick * (selectedIndex), 0f);

			movement = SmoothRotation(quart);
			StartCoroutine(movement);
			StartCoroutine(setCanRotate(rotateTime * 0.6f));
		}*/

        private IEnumerator SmoothRotation(Quaternion end)
        {
			Quaternion startRot = transform.rotation;
			float t = 0;

			while (t < 1)
            {
				t += Time.deltaTime / rotateTime;
				t = Mathf.Clamp01(t);

				transform.rotation = Quaternion.Lerp(startRot, end, Easing.Quadratic.InOut(t));
                yield return null;
            }
        }

		private IEnumerator setCanRotate(float t) {
			canRotate = false;
			yield return new WaitForSeconds(t);
			canRotate = true;
		}
    }
}
