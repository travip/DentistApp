using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class MenuManager : MonoBehaviour
    {

        public static MenuManager instance { get; private set; }

        public Menu currentMenu;
        public Menu prevMenu;

        public MenuItem selectedItem;

        public float moveTime = 0.1f;
        private float inverseMoveTime;
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

            inverseMoveTime = 1f / moveTime;
        }

        // Update is called once per frame
        void Update()
        {

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
            Vector3 newPosition = transform.position + Vector3.down * 8;
            currentMenu = selectedItem.menuOnSelect;
            StartCoroutine(SmoothMovement(newPosition));
        }

        private IEnumerator SmoothMovement(Vector3 end)
        {
            isMoving = true;
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
                transform.position = newPosition;
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                yield return null;
            }
            isMoving = false;

        }
    }
}
