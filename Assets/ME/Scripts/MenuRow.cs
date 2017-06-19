﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CylinderMenu
{
    public class MenuRow : MonoBehaviour
    {
		// Object links
		public GameObject BackSelectorPrefab, NextPageSelectorPrefab, PreviousPageSelectorPrefab;
		public List<MenuItem> menuItems;
		private Transform navButtons;

		// Parameters

		[Header("Navigation")]
		public float turnTick = 10f;
		public float rotateTime = 0.1f;
		public float turnAccel;
		public float turnRateMax;
		public float turnFriction;
		
		[Header("Image layout")]
		public int maxRows = 2;
		public int maxColumns = 5;
		public float itemSize = 20f;
		public float gapBetweenItems = 1f;
		private float radius;
		private int pages = 1;


		// Runtime variables
		[HideInInspector]
		public MenuRow belowRow;
		[HideInInspector]
		public bool canRotate = true;
		private float turnRate = 0f;
		private float targetTurnRate = 0f;
		private bool turning = false;
		
		//private int currentPage = 0;

		IEnumerator movement;

		//public MenuItem selectedItem
		//{
		//	get { return menuItems[selectedIndex]; }
		//}
		

		[HideInInspector]
		public MenuItem selectedItem;

        // Dynamic creation of menu
        public void InitializeMenu(MenuRow parentRow, float _radius, int _rows, int _columns, float _itemSize, float _gapBetweenItems)
        {
			radius = _radius;
			maxRows = _rows;
			maxColumns = _columns;
			itemSize = _itemSize;
			gapBetweenItems = _gapBetweenItems;

			transform.Find("mesh").localScale = new Vector3(radius+1f, radius+1f, 3f);

			gameObject.SetActive(true);

			if (parentRow != null)
			{
				belowRow = parentRow;
				menuItems = belowRow.selectedItem.subMenuItems;
				PositionMenuItems();
				CreateMainButtons();
			} else
			{
				PositionMenuItems();
			}

        }

		
		void Update() {

			if (turnRate != 0f) {
				if (!turning) {
					turnRate *= turnFriction;
				}
				
				transform.Rotate(0f, turnRate, 0f);

				turning = false;
			}
		}




		public void RecalculateRow(float _radius, int _rows, int _columns, float _itemSize, float _gapBetweenItems) {
			radius = _radius;
			maxRows = _rows;
			maxColumns = _columns;
			itemSize = _itemSize;
			gapBetweenItems = _gapBetweenItems;

			transform.Find("mesh").localScale = new Vector3(radius + 1f, radius + 1f, 3f);

			PositionMenuItems();

			if (belowRow != null) {
				Destroy(navButtons.gameObject);
				CreateMainButtons();
			}
		}

		public void PositionMenuItems() {
			// Need to track index for proper positioning

			// Calculate the number of columns required
			int numRows = maxRows;
			int numColumns = Mathf.CeilToInt((float)menuItems.Count / (float)numRows);

			pages = Mathf.CeilToInt((float)menuItems.Count / (float)(maxColumns * maxRows));

			if (pages == 0)
				pages = 1;
			
			if (numColumns > maxColumns) {
				numColumns = maxColumns;
				// Only display a certain amount of columns
			}

			// Calculate how far many degrees around the circle each image appears after the last.

			float degreesPerUnit = 360f / (2f * Mathf.PI * radius);
			float picRotDiffX = degreesPerUnit * itemSize;
			float picRotDiffY = picRotDiffX * 0.981f; // 0.981 is 'y' dimension from blender
			
			picRotDiffX += degreesPerUnit * gapBetweenItems;
			picRotDiffY += degreesPerUnit * gapBetweenItems;

			// The columns are centered based on how many columns there are. 
			// The first column's rotation therefore depends on how many columns there are
			float startRotY = ((numColumns / 2f) - 0.5f) * -picRotDiffY;
			float startRotX = ((numRows / 2f) - 0.5f) * -picRotDiffX;

			int col = 0, row = 0;
			for (int i = 0; i < menuItems.Count; i++) {
				// Some of this might be able to be done in MenuItem or set beforehand

				menuItems[i].AddToMenuRow(transform, radius, Quaternion.Euler(new Vector3(startRotX + picRotDiffX * row, startRotY + picRotDiffY * col, 0f)), itemSize);

				row++;
				
				if (row >= numRows)
				{
					row = 0;
					col++;
					if (col >= numColumns)
					{
						break;
					}
				}
					
				
			}
		}

		private void CreateMainButtons() {
			// Create back button and navigation buttons (left/right)
			navButtons = new GameObject().transform;
			navButtons.parent = MenuManager.instance.transform;
			navButtons.transform.position = new Vector3(0f, transform.position.y - 2f, radius);

			Instantiate(BackSelectorPrefab, navButtons);
			if (pages > 1)
			{
				Instantiate(NextPageSelectorPrefab, navButtons).transform.localPosition = new Vector3(2f, 0f, 0f);
				Instantiate(PreviousPageSelectorPrefab, navButtons).transform.localPosition = new Vector3(-2f, 0f, 0f);
			}
			
		}

        // Remove menu items - put them in the faraway land where they wont get in our way
        public void TerminateMenu(Transform container)
        {
            foreach(MenuItem item in menuItems)
            {
                item.transform.SetParent(container);
            }

			menuItems = null;

			Destroy(navButtons.gameObject);
			Destroy(gameObject);
        }


		public MenuItem NextImage() {
			// Return the next viewable image in the list of menu items
			int current = menuItems.IndexOf(selectedItem);

			do {
				current++;
				if (current >= menuItems.Count) {
					current = 0;
				}
				selectedItem = menuItems[current];
			} while (selectedItem.selectAction != MenuManager.SelectAction.imageView);

			// Also make sure the row is rotated to that item
			RotateToSelectedItem();

			return selectedItem;
		}

		public MenuItem PreviousImage () {
			// Return the next viewable image in the list of menu items
			int current = menuItems.IndexOf(selectedItem);

			do {
				current--;
				if (current <= 0) {
					current = menuItems.Count - 1;
				}
				selectedItem = menuItems[current];
			} while (selectedItem.selectAction != MenuManager.SelectAction.imageView);

			// Also make sure the row is rotated to that item
			RotateToSelectedItem();
			
			return selectedItem;
		}

		private void RotateToSelectedItem() {
			transform.rotation = Quaternion.Euler(0f, -selectedItem.transform.localRotation.eulerAngles.y, 0f);
		}


		public void TurnLeft(float percent) {
			targetTurnRate = percent * turnRateMax;
			
			Turn();
		}

		public void TurnRight (float percent) {
			targetTurnRate = percent * -turnRateMax;

			Turn();
		}

		private void Turn() {
			if (turnRate > targetTurnRate) {
				turnRate -= turnAccel * Time.deltaTime;
				if (turnRate < targetTurnRate)
					turnRate = targetTurnRate;
			} else {
				turnRate += turnAccel * Time.deltaTime;
				if (turnRate > targetTurnRate)
					turnRate = targetTurnRate;
			}

			turning = true;
		}

		public void MoveRight()
        {
			if (!canRotate)
				return;

			RotateView(-turnTick);
        }

        public void MoveLeft()
        {
			if (!canRotate)
				return;

			RotateView(turnTick);
		}

		private void RotateView(float degrees) {
			if (movement != null)
				StopCoroutine(movement);

			//Quaternion quart = Quaternion.Euler(0f, -turnTick * (selectedIndex), 0f);
			Quaternion quart = transform.rotation * Quaternion.Euler(Vector3.up * degrees);
			
			movement = SmoothRotation(quart);
			StartCoroutine(movement);
			StartCoroutine(setCanRotate(rotateTime * 0.6f));
		}

        private IEnumerator SmoothRotation(Quaternion end)
        {
			Quaternion startRot = transform.rotation;
			float t = 0;

			while (t < 1)
            {
				t += Time.deltaTime / rotateTime;
				t = Mathf.Clamp01(t);

				transform.rotation = Quaternion.Lerp(startRot, end, Easing.Quadratic.InOut(t));
                yield return null;
            }
        }

		private IEnumerator setCanRotate(float t) {
			canRotate = false;
			yield return new WaitForSeconds(t);
			canRotate = true;
		}
    }
}
