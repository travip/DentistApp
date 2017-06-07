using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu {

    public class MenuItem : MonoBehaviour {

        // Get an explicit refernce to the submenu transform so we can use it in dynamic menu creation
        public List<MenuItem> subMenuItems;

        public string itemName;
        public MenuRow menuOnSelect;

        // Initialize List
        private void Awake()
        {
            subMenuItems = new List<MenuItem>();
        }

        private void Start()
        {
            // Get each MenuItem from the SubMenu container
            foreach(Transform sub in transform.GetChild(0))
            {
                subMenuItems.Add(sub.GetComponent<MenuItem>());
            }
        }
    }
}
