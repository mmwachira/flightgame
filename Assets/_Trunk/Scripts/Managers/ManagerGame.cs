using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerGame : MonoBehaviour
    {
        public static ManagerGame Instance { get; private set; }

        private float _highScore = 0;
        private bool _gameOver;
        private bool _gamePaused;


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

        public void Start()
        {
            ResetGame();
            ManagerEducationalContent.Instance.LoadEducationalQuestions();
            ManagerLevel.Instance.Setup();
        }

        void Update()
        {
            if (_gameOver)
            {

                ManagerLevel.Instance.EndGame();
                ManagerUI.Instance.ShowGameOverScreen();
                ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu);
            }

            if (_gamePaused)
            {
                Time.timeScale = 0;
                ManagerUI.Instance.ShowMenu();

            }
        }

        private void ResetGame()
        {
            _gameOver = false;
            _gamePaused = false;
            ManagerUI.Instance.ShowStartGameScreen();
            ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu);
        }

        public void StartGame()
        {
            Time.timeScale = 1;
            ManagerUI.Instance.ShowGameplayHUD();
            ManagerUI.Instance.UpdateHighScore((int)_highScore);
            ManagerLevel.Instance.StartGame();
            ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicGameplay);
        }

        public void PauseGame()
        {
            _gamePaused = true;
        }

        public void ResumeGame()
        {
            _gamePaused = false;
            Time.timeScale = 1;
            ManagerUI.Instance.ShowGameplayHUD();


        }

        public void GameOver()
        {
            _gameOver = true;
            _highScore = ManagerLevel.Instance.m_Score;
            ManagerUI.Instance.UpdateFinalScore((int)ManagerLevel.Instance.TotalRunDistance);

        }

        public void ReplayGame()
        {
            //PlayerData.instance.InsertScore((int)ManagerLevel.Instance.m_Score);
            //PlayerData.instance.Save();
            ResetGame();

        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}