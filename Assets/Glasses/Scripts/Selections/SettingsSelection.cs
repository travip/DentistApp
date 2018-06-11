using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class SettingsSelection : MonoBehaviour, iSelectable
	{
		private SettingsItem settingsItem;
		public float valueChange;

		void Start()
		{
			settingsItem = transform.parent.parent.GetComponent<SettingsItem>();
		}

		public void Select ()
		{
			Settings.instance.ChangeSetting(settingsItem.setting, valueChange);
		}
	}
}
