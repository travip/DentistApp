using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuRow : MonoBehaviour
    {

		public List<MenuItem> menuItems;
        public int selectedItem = 0;

        public float rotateTime = 0.1f;
        private bool isRotating = false;

		IEnumerator movement;

        public void MoveRight()
        {
			if (selectedItem < menuItems.Count - 1)
            {
				selectedItem++;
				MoveToSelectedItem();
            }
        }

        public void MoveLeft()
        {
            if (selectedItem > 0)
            {
				selectedItem--;
				MoveToSelectedItem();
			}
        }

		private void MoveToSelectedItem() {

			if (movement != null)
				StopCoroutine(movement);

			Quaternion quart = Quaternion.Euler(90f, 0f, 45f * (selectedItem));

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
