using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuRow : MonoBehaviour
    {

		public List<MenuItem> menuItems;
        private int selectedIndex = 0;

        public MenuRow belowRow;

        public float rotateTime = 0.1f;
        private bool isRotating = false;

		IEnumerator movement;

        public MenuItem selectedItem
        {
            get { return menuItems[selectedIndex]; }
        }

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
			for (int i = 0; i < menuItems.Count; i++) {
				// Some of this might be able to be done in MenuItem or set beforehand
				menuItems[i].transform.SetParent(transform);
				menuItems[i].transform.localPosition = Vector3.zero;
				menuItems[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 45f * i, 0));
				menuItems[i].gameObject.SetActive(true);
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
			if (selectedIndex < menuItems.Count - 1)
            {
                selectedIndex++;
				MoveToSelectedItem();
            }
        }

        public void MoveLeft()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
				MoveToSelectedItem();
			}
        }

		private void MoveToSelectedItem() {

			if (movement != null)
				StopCoroutine(movement);

			Quaternion quart = Quaternion.Euler(0f, -45f * (selectedIndex), 0f);

			movement = SmoothRotation(quart);
			StartCoroutine(movement);
		}

        private IEnumerator SmoothRotation(Quaternion end)
        {
            isRotating = true;

			Quaternion startRot = transform.rotation;
			float t = 0;

			while (t < 1)
            {
				t += Time.deltaTime / rotateTime;
				t = Mathf.Clamp01(t);

				transform.rotation = Quaternion.Lerp(startRot, end, Easing.Quadratic.InOut(t));
                yield return null;
            }

			isRotating = false;
        }
    }
}
