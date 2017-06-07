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

        public MenuItem selected;

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
    }
}
