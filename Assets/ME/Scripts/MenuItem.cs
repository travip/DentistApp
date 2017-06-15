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
		public GameObject SelectorPrefab;

        public string itemName;
		public Texture menuPic;
		public MenuManager.SelectAction selectAction;

		private GameObject pic;
		private GameObject selector;

		[HideInInspector]
		public Texture FullSizedPic;

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

			// Instantiate the mesh that holds the menu picture
			pic = Instantiate(PicPrefab, transform).gameObject;
			pic.transform.Find("mesh").GetComponent<Renderer>().material.mainTexture = menuPic;

			// Instantiate selector
			selector = Instantiate(SelectorPrefab, transform).gameObject;

			gameObject.SetActive(false);
        }

		public void AddToMenuRow(Transform row, float distance, Quaternion rotation, float size)
		{
			transform.SetParent(row);
			transform.localPosition = Vector3.zero;
			transform.localRotation = rotation;

			pic.transform.localPosition = new Vector3(0f, 0f, distance);
			pic.transform.localScale = new Vector3(size, size, size);

			// slight magic number for the y position of the selector
			selector.transform.localPosition = new Vector3(0f, pic.transform.localScale.z / -16f, distance);

			gameObject.SetActive(true);
		}
    }
}
