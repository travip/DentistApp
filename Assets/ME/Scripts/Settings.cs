using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{

	public class Settings : MonoBehaviour
	{
		public static Settings instance { get; private set; }

		public enum Setting
		{
			gestureSensitivityX,
			gestureSensitivityY,
			movementSensitivity,
			rowScrollSpeed,
			deadzoneSize
		}

		private void Awake ()
		{
			if (instance == null || instance == this)
				instance = this;
			else
				Destroy(this);
		}

		public void ChangeSetting(Setting setting, float amount) {
			switch (setting) {
				case Setting.gestureSensitivityX:
					ChangeGestureSensitivityX(amount);
					break;
				case Setting.gestureSensitivityY:
					ChangeGestureSensitivityY(amount);
					break;
				case Setting.movementSensitivity:
					ChangeMovementSensitivity(amount);
					break;
				case Setting.rowScrollSpeed:
					ChangeScrollSpeed(amount);
					break;
				case Setting.deadzoneSize:
					ChangeDeadzoneSize(amount);
					break;
				default:
					break;
			}
		}

		private void ChangeGestureSensitivityX (float amount)
		{
			InputManager.instance.moveThreshold.x += amount;
		}

		private void ChangeGestureSensitivityY (float amount)
		{
			InputManager.instance.moveThreshold.y += amount;
		}

		private void ChangeMovementSensitivity (float amount)
		{
			InputManager.instance.scrollSpeed += amount;
		}

		private void ChangeScrollSpeed (float amount)
		{
			MenuManager.instance.turnThresholdMin += amount;
		}

		private void ChangeDeadzoneSize (float amount)
		{
			MenuManager.instance.turnThresholdMin += amount;
		}

	}
}
