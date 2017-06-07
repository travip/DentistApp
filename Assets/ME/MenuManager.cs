using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuManager : MonoBehaviour
    {

        public static MenuManager instance { get; private set; }

        public MenuRow currentMenu;
        public MenuRow prevMenu;

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

			Vector3 newPosition = transform.position + Vector3.down * 8;
            currentMenu = currentMenu.menuItems[currentMenu.selectedItem].menuOnSelect;

            StartCoroutine(SmoothMovement(newPosition));
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
