﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class Menu : MonoBehaviour
    {
        private MenuManager menuManager;

        public List<MenuItem> menuItems;
        public int selectedItem = 0;

        public float rotateTime = 0.1f;
        private bool isRotating = false;

        private void Start()
        {
            menuManager = MenuManager.instance;
        }

        public void MoveRight()
        {
            if (!isRotating)
            {
                if (selectedItem < menuItems.Count - 1)
                {
                    Quaternion quart = Quaternion.Euler(0f,0f, 45f * (++selectedItem));
                    menuManager.selectedItem = menuItems[selectedItem];
                    StartCoroutine(SmoothRotation(quart));
                }
            }
        }

        public void MoveLeft()
        {
            if (!isRotating)
            {
                if (selectedItem > 0)
                {
                    Quaternion quart = Quaternion.Euler(0f, 0f, 45f * (--selectedItem));
                    menuManager.selectedItem = menuItems[selectedItem];
                    StartCoroutine(SmoothRotation(quart));
                }
            }
        }

        private IEnumerator SmoothRotation(Quaternion end)
        {
            isRotating = true;
            while (Quaternion.Angle(transform.rotation, end) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, end, Time.time * rotateTime);
                yield return null;
            }
            isRotating = false;
        }
    }
}
