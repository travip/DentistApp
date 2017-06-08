using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu {

    public class MenuItem : MonoBehaviour {

        // Get an explicit refernce to the submenu transform so we can use it in dynamic menu creation
		[HideInInspector]
        public List<MenuItem> subMenuItems;

		[HideInInspector]
		public MenuRow menuOnSelect;

		public GameObject PicPrefab;

        public string itemName;
		public Texture menuPic;
		public MenuManager.SelectAction selectAction;

		private GameObject pic;

        // Initialize List
        private void Awake()
        {
            subMenuItems = new List<MenuItem>();
        }

        private void Start()
        {
            // Get each MenuItem from the SubMenu container
            foreach(Transform sub in transform)
            {
                subMenuItems.Add(sub.GetComponent<MenuItem>());
            }

			pic = Instantiate(PicPrefab, transform).gameObject;
			pic.GetComponent<Renderer>().material.mainTexture = menuPic;

			gameObject.SetActive(false);
        }
    }
}
