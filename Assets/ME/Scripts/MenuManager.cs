using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuManager : MonoBehaviour
    {

        public static MenuManager instance { get; private set; }

        public MenuRow currentMenu;
        public MenuRow belowMenu;

		private MenuRow TempAboveMenu;

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
            Debug.Log("Add Listener");
            InputManager.instance.goRight.AddListener(MoveMenuRight);
            InputManager.instance.goLeft.AddListener(MoveMenuLeft);
            InputManager.instance.goUp.AddListener(MoveMenuUp);
			InputManager.instance.goDown.AddListener(MoveMenuDown);
		}

        public void MoveMenuRight()
        {
            currentMenu.MoveRight();
        }

        public void MoveMenuLeft()
        {
            currentMenu.MoveLeft();
        }

        public void MoveMenuUp()
        {
			if (isMoving)
				return;

			MenuRow newMenu = currentMenu.menuItems[currentMenu.selectedItem].menuOnSelect;

			if (newMenu == null)
				return;

			belowMenu = currentMenu;
			currentMenu = newMenu;

			Debug.Log(currentMenu);
			currentMenu.gameObject.SetActive(true);

			Vector3 newPosition = transform.position;
			newPosition.y = -currentMenu.transform.localPosition.y;

			StartCoroutine(SmoothMovement(newPosition));
        }

		public void MoveMenuDown()
		{
			if (isMoving)
				return;
			if (belowMenu == null)
				return;

			TempAboveMenu = currentMenu;
			currentMenu = belowMenu;
			belowMenu = null;

			Vector3 newPosition = transform.position;
			newPosition.y = -currentMenu.transform.localPosition.y;

			StartCoroutine(BackMenu(newPosition));
		}

		private IEnumerator BackMenu(Vector3 end) {
			yield return StartCoroutine(SmoothMovement(end));
			TempAboveMenu.gameObject.SetActive(false);
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
