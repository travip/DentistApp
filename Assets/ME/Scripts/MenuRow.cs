using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuRow : MonoBehaviour
    {
		private float turnTick = 10f;
		private float picSpacingX = 5f;
		private float picSpacingY = 4f;
		public List<MenuItem> menuItems;
        //private int selectedIndex = 0;

        public MenuRow belowRow;

        public float rotateTime = 0.1f;
        public bool canRotate = true;

		public int maxRows = 2;
		public int maxColumns = 6;

		IEnumerator movement;

		
		//public MenuItem selectedItem
		//{
		//	get { return menuItems[selectedIndex]; }
		//}
		

		[HideInInspector]
		public MenuItem selectedItem;

        // Dynamic creation of menu
        public void InitializeMenu(MenuRow parentRow)
        {
			belowRow = parentRow;
			menuItems = belowRow.selectedItem.subMenuItems;
			gameObject.SetActive(true);

			PositionMenuItems();
        }

		public void PositionMenuItems() {
			// Need to track index for proper positioning

			// Calculate the number of columns required
			int numRows = maxRows;
			int numColumns = Mathf.CeilToInt((float)menuItems.Count / (float)numRows);

			if (numColumns > maxColumns) {
				// Only display a certain amount of columns
			}


			// The columns are centered based on how many columns there are. 
			// The first column's rotation therefore depends on how many columns there are
			float startRotY = ((numColumns / 2f) - 0.5f) * -picSpacingX;
			float startRotX = ((numRows / 2f) - 0.5f) * -picSpacingY;

			float yDif = -2f;

			int col = 0, row = 0;
			for (int i = 0; i < menuItems.Count; i++) {
				// Some of this might be able to be done in MenuItem or set beforehand
				menuItems[i].transform.SetParent(transform);
				menuItems[i].transform.localPosition = Vector3.zero;
				menuItems[i].transform.localRotation = Quaternion.Euler(new Vector3(startRotX + picSpacingY * row, startRotY + picSpacingX * col, 0));
				menuItems[i].gameObject.SetActive(true);

				row++;
				

				if (row >= numRows)
				{
					row = 0;
					col++;
				}
					
				
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
