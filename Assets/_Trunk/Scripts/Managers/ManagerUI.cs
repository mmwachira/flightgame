using FlightGame.Questions;
using TMPro;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerUI : MonoBehaviour
    {
        public static ManagerUI Instance { get; private set; }

        [SerializeField] private GameObject _gameplayHUD;
        [SerializeField] private GameObject _viewStartGame;
        [SerializeField] private GameObject _viewGameOver;
        [SerializeField] private GameObject _viewLeaderBoard;
        [SerializeField] private GameObject _viewMenu;
        [SerializeField] private GameObject _viewQuestion;

        [SerializeField] private GameObject _life01;
        [SerializeField] private GameObject _life02;
        [SerializeField] private GameObject _life03;
        [SerializeField] private TextMeshProUGUI _distance;
        [SerializeField] private TextMeshProUGUI _collected;
        [SerializeField] private TextMeshProUGUI t_distance;
        [SerializeField] private TextMeshProUGUI _highScore;
        [SerializeField] private TextMeshProUGUI _question;

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


        public void ToggleHUD(bool showHUD)
        {
            _gameplayHUD.SetActive(showHUD);
        }

        public void ShowStartGameScreen()
        {
            ToggleHUD(false);
            _viewStartGame.SetActive(true);
            _viewGameOver.SetActive(false);
            _viewMenu.SetActive(false);
            _viewQuestion.SetActive(false);
        }

        public void ShowGameOverScreen()
        {
            ToggleHUD(false);
            _viewStartGame.SetActive(false);
            _viewGameOver.SetActive(true);
            _viewLeaderBoard.SetActive(true);
            _viewMenu.SetActive(false);
            _viewQuestion.SetActive(false);
        }

        public void ShowGameplayHUD()
        {
            ToggleHUD(true);
            _viewStartGame.SetActive(false);
            _viewGameOver.SetActive(false);
            _viewMenu.SetActive(false);
            _viewQuestion.SetActive(false);
        }

        public void ShowMenu()
        {
            ToggleHUD(false);
            _viewStartGame.SetActive(false);
            _viewGameOver.SetActive(false);
            _viewMenu.SetActive(true);
            _viewQuestion.SetActive(false);
        }

        public void UpdateLivesDisplay(int currentLives)
        {
            _life03.SetActive(currentLives > 2);
            _life02.SetActive(currentLives > 1);
            _life01.SetActive(currentLives > 0);
        }

        public void Reset()
        {
            UpdateDistance(0);
            UpdateCollected(0);
        }

        public void UpdateDistance(int value)
        {
            _distance.text = value.ToString();
        }

        public void UpdateCollected(int value)
        {
            _collected.text = value.ToString();
        }

        public void UpdateFinalScore(int value)
        {
            t_distance.text = "GAME OVER! YOUR SCORE WAS: " + value.ToString() + ". WANT TO PLAY AGAIN?";
        }

        public void UpdateHighScore(int value)
        {

            _highScore.text = "BEST: " + value.ToString();
        }

        public void UpdateQuestion(Question question)
        {
            ToggleHUD(true);
            _viewQuestion.SetActive(true);
            _question.text = question.questionText;
        }

        public void UpdateAnswer()
        {
            ToggleHUD(true);
            _viewQuestion.SetActive(false);
        }
    }
}
