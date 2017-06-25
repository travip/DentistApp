using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class TransitionItem : MenuItem
	{
		public GameObject SelectorPrefab;
		private Transform selector;

		override protected void Awake() {
			base.Awake();

			if (selectAction != MenuManager.SelectAction.none) {
				selector = Instantiate(SelectorPrefab, selectors).transform;
			}
		}

		public override void AddToMenuRow (Transform row, float distance, Quaternion rotation, Vector3 scale) {
			base.AddToMenuRow(row, distance, rotation, scale);

			// slight magic number for the y position of the selector
			if (selector != null)
				selector.localPosition = new Vector3(0f, scale.y * -0.14f, 0f);
		}
	}
}