using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CylinderMenu
{

	public class Settings : MonoBehaviour
	{
		public static Settings instance { get; private set; }

		public MenuSetting senseX, senseY, look, scroll, deadzone, viewResetTime;

		public enum Setting
		{
			gestureSensitivityX,
			gestureSensitivityY,
			movementSensitivity,
			rowScrollSpeed,
			deadzoneSize,
			viewResetTime
		}

		private void Awake ()
		{
			if (instance == null || instance == this)
			{
				instance = this;
			}
			else
				Destroy(this);
		}

		void Start() {
			InitialiseValues();
		}

		private void InitialiseValues() {
			senseX.Set(InputManager.instance.moveThreshold.x);
			senseY.Set(InputManager.instance.moveThreshold.y);
			look.Set(InputManager.instance.scrollSpeed);
			scroll.Set(MenuRow.turnRateMax);
			deadzone.Set(MenuManager.instance.turnThresholdMin);
			viewResetTime.Set(InputManager.instance.preventMovementTime);
		}


		public void ChangeSetting(Setting setting, float amount) {
			switch (setting) {
				case Setting.gestureSensitivityX:
					InputManager.instance.moveThreshold.x = senseX.Tick(amount);
					break;
				case Setting.gestureSensitivityY:
					InputManager.instance.moveThreshold.y = senseY.Tick(amount);
					break;
				case Setting.movementSensitivity:
					InputManager.instance.scrollSpeed = look.Tick(amount);
					break;
				case Setting.rowScrollSpeed:
					MenuRow.turnRateMax = scroll.Tick(amount);
					break;
				case Setting.deadzoneSize:
					MenuManager.instance.turnThresholdMin = deadzone.Tick(amount);
					break;
				case Setting.viewResetTime:
					InputManager.instance.preventMovementTime = viewResetTime.Tick(amount);
					break;
				default:
					break;
			}
		}
	}

	[System.Serializable]
	public class MenuSetting
	{
		public TextMesh text;
		public float min, max, tick;
		private float current;

		public void Set(float value) {
			current = value;
			UpdateText();
		}

		public float Tick(float mult) {
			current += (tick * mult);
			current = Mathf.Clamp(current, min, max);
			UpdateText();
			return current;
		}

		private void UpdateText () {
			string textRouding = "F0";
			if (tick < 1)
				textRouding = "F1";
			
			text.text = current.ToString(textRouding);
		}
	}

}
