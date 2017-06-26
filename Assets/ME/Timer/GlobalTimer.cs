using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CylinderMenu
{
	public class GlobalTimer : MonoBehaviour
	{
		public static GlobalTimer instance { get; private set; }

		public Text timerText;
        public Color defaultColor;
		public float totalTime;

		private float currentTime;
		private bool timing = false;
		private int currentDisplayNumber;

		private void Awake ()
		{
			if (instance == null || instance == this)
			{
				instance = this;
				Initialise();
			}
				
			else
				Destroy(this);
		}
		void Initialise()
		{
			timerText.gameObject.SetActive(false);
		}

		void Update ()
		{
			if (timing)
			{
				currentTime -= Time.deltaTime;

				if (currentDisplayNumber - currentTime > 1f)
				{
					UpdateText();
				}

				if (currentTime <= 0)
				{
                    currentTime = 0;
					TimesUp();
				}
			}
		}

		private void UpdateText ()
		{
			int mins = Mathf.FloorToInt(currentTime / 60f);
			int secs = Mathf.CeilToInt(currentTime % 60f);

			currentDisplayNumber = mins * 60 + secs;

			timerText.text =
				(mins < 10 ? "0" + mins.ToString() : mins.ToString()) +
				":" +
				(secs < 10 ? "0" + secs.ToString() : secs.ToString());
		}

		public void StartTimer (float time)
		{
            Debug.Log("Start Timing");
            timerText.color = defaultColor;
			timerText.gameObject.SetActive(true);
            totalTime = time;
            currentTime = time;
            UpdateText();
			timing = true;
		}

		private void TimesUp ()
		{
            UpdateText();
            timerText.color = Color.red;
            timing = false;
		}

		public void PauseTimer ()
		{
			timing = false;
		}

        public void ResumeTimer()
        {
            timing = true;
        }

		public void ResetTimer ()
		{
			timerText.gameObject.SetActive(false);
			timing = false;
			currentTime = totalTime;
		}
	}
}