using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class SettingsItem : MenuItem
	{
		public GameObject SelectorPrefab;

		public string label;
		public Settings.Setting setting;

		public float currentValue;
		
		private SettingsSelection selectorUp;
		private SettingsSelection selectorDown;

		protected override void Awake () {
			base.Awake();

			selectorUp = Instantiate(SelectorPrefab, selectors).GetComponent<SettingsSelection>();
			selectorDown = Instantiate(SelectorPrefab, selectors).GetComponent<SettingsSelection>();
			selectorUp.valueChange = 1f;
			selectorDown.valueChange = -1f;
		}

		public override void AddToMenuRow (Transform row, float distance, Quaternion rotation, Vector3 scale) {
			base.AddToMenuRow(row, distance, rotation, scale);

			// slight magic number for the y position of the selector
			selectorUp.transform.localPosition = new Vector3(0f, scale.y * 0.3f, 0f);
			selectorDown.transform.localPosition = new Vector3(0f, scale.y * -0.3f, 0f);
		}

		public void ChangeValue (float newValue)
		{
			currentValue = newValue;
			Debug.Log(currentValue);
		}
	}
}
