using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CylinderMenu
{
    public class PipTimer : MonoBehaviour
    {
        public static PipTimer instance { get; private set; }

        public Text timerText;
        public Text label;
        public Color defaultColor;

        [SerializeField]
        private float currentTime;
        private bool timing = false;
        private int currentDisplayNumber;

        private void Awake()
        {
            if(instance == null || instance == this)
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

        void Update()
        {
            if(timing)
            {
                currentTime += Time.deltaTime;
                UpdateText();
                if(currentTime <= 0)
                {
                    currentTime = 0;
                }
            }
        }

        private void UpdateText()
        {
            int mins = Mathf.FloorToInt((currentTime + 0.99f) / 60f); // Adding 0.99 makes it so that it shows /rounds the higher number 
            int secs = Mathf.FloorToInt((currentTime + 0.99f) % 60f); // i.e. it won't hit 0 until actually at 0 (rather than as soon as it's below 1)

            currentDisplayNumber = mins * 60 + secs;

            timerText.text =
                (mins < 10 ? "0" + mins.ToString() : mins.ToString()) +
                ":" +
                (secs < 10 ? "0" + secs.ToString() : secs.ToString());
            if(currentTime > 60)
            {
                timerText.color = Color.red;
                label.color = Color.red;
            }
        }

        public void LoadTimer()
        {
            timerText.gameObject.SetActive(true);
            StartCoroutine(Fade(0f, 1f, Constants.Transitions.FadeTime));
            StartTimer();
        }

        public void UnloadTimer()
        {
            StartCoroutine(Fade(1f, 0f, Constants.Transitions.FadeTime));
        }

        public void StartTimer()
        {
            Debug.Log("Start Timing");
            timerText.color = defaultColor;
            timerText.gameObject.SetActive(true);
            currentTime = 0;
            UpdateText();
            timing = true;
        }

        public void ResetTimer()
        {
            currentTime = 0;
            timerText.color = defaultColor;
            label.color = defaultColor;
        }

        public void StopTimer()
        {
            timing = false;
            currentTime = 0;
            timerText.color = defaultColor;
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float totalTime)
        {
            float t = 0;
            float alpha = startAlpha;
            Color col = timerText.color;

            while(t < totalTime)
            {
                t += Time.deltaTime;
                col.a = Mathf.Lerp(startAlpha, endAlpha, t / totalTime);
                timerText.color = col;
                label.color = col;
                yield return null;
            }
        }
    }
}