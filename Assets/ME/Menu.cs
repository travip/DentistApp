using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
    public class Menu : MonoBehaviour
    {
        public List<MenuItem> menuItems;
        private int selectedItem = 0;

        public float rotateTime = 1f;
        private float inverseRotateTime;
        private bool isRotating = false;

        private void Awake()
        {
        }

        private void Start()
        {
            inverseRotateTime = 1f / rotateTime;
        }

        public void MoveRight()
        {
            if (!isRotating)
            {
                Quaternion quart = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 45f);
                StartCoroutine(SmoothRotation(quart));
            }
        }

        public void MoveLeft()
        {
            if (!isRotating)
            {
                Quaternion quart = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -45f);            
                StartCoroutine(SmoothRotation(quart));
            }
        }

        private IEnumerator SmoothRotation(Quaternion end)
        {
            while (Quaternion.Angle(transform.rotation, end) > 0.1f)
            {
                Debug.Log(Quaternion.Angle(transform.rotation, end));
                transform.rotation = Quaternion.Slerp(transform.rotation, end, Time.time * rotateTime);
                yield return null;
            }
        }
    }
}
