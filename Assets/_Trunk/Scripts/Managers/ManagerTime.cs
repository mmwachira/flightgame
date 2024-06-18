using System;
using System.Collections;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerTime : MonoBehaviour
    {
        public static ManagerTime Instance { get; private set; }

        [SerializeField] private float _slowdownFactor = .2f;
        [SerializeField] private float _slowdownLength = 4f;
        private bool isSlowingDown = false;
        private float originalTimeScale;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!isSlowingDown)
            {
                Time.timeScale += 1f / _slowdownLength * Time.unscaledDeltaTime;
                Time.timeScale = Math.Clamp(Time.timeScale, 0f, 1f);
            }

        }
        public void StartSlowMotion(float transitionTime)
        {
            StartCoroutine(SlowDownCoroutine(transitionTime));
        }

        private IEnumerator SlowDownCoroutine(float transitionTime)
        {
            isSlowingDown = true;
            originalTimeScale = Time.timeScale;
            float elapsedTime = 0f;

            while (elapsedTime < transitionTime)
            {
                Time.timeScale = Mathf.Lerp(originalTimeScale, _slowdownFactor, elapsedTime / transitionTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = _slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            //Maintain slow motion for a set duration
            yield return new WaitForSecondsRealtime(_slowdownLength);

            elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                Time.timeScale = Mathf.Lerp(_slowdownFactor, originalTimeScale, elapsedTime / transitionTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            isSlowingDown = false;
        }

        public void DoSlowMotion()
        {
            Time.timeScale = _slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }
}