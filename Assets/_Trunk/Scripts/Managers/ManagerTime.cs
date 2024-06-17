using System;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerTime : MonoBehaviour
    {
        public static ManagerTime Instance { get; private set; }

        [SerializeField] private float _slowdownFactor = .2f;
        [SerializeField] private float _slowdownLength = 5f;

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
            Time.timeScale += (1f / _slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Math.Clamp(Time.timeScale, 0f, 1f);
        }

        public void DoSlowMotion()
        {
            Time.timeScale = _slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        } 
    }
}