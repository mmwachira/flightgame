using System.Collections.Generic;
using FlightGame.Players;
using FlightGame.Questions;
using TMPro;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerQuestions : MonoBehaviour
    {
        public static ManagerQuestions Instance { get; private set; }

        public string[] questions => _question;
        public string[] answers => _answer;
        public PlayerController playerController;

        [SerializeField] private GameObject[] answerRings;
        [SerializeField] private float _ringSpawnDistance = 0.5f; // Distance ahead of the player to spawn rings
        [SerializeField] private float _ringSpacing = 0.5f; // Spacing between rings
        [SerializeField] private float _middleRingYOffset = 1.0f;
        private string[] _answer;
        private string[] _question;
        private List<Question> _questions;
        private HashSet<int> _askedQuestions;
        private int _currentQuestionId;
        private int _correctAnswerIndex;

        private Transform _levelContainer;
        private PlayerController _playerController;


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

        void Start()
        {
            _playerController = ManagerLevel.Instance.playerController;
            _levelContainer = ManagerLevel.Instance.transform;

            _questions = new List<Question>
       {
        new Question(0, "What color is the plane?", new string[]{"Red", "Blue", "Green" }, 0),
        new Question(1, "When you make a new friend, how do you feel?", new string[] { "Happy", "Sad", "Nervous" }, 0),
        new Question(2, "When you cry, how do you feel?", new string[] { "Sad", "Relieved", "Comforted" }, 0)
       };

            _askedQuestions = new HashSet<int>();
            _currentQuestionId = -1;

        }

        public void AskQuestion()
        {

            _currentQuestionId = GetNextQuestionId();
            if (_currentQuestionId != -1)
            {
                DisplayQuestion(_currentQuestionId);
            }
            else
            {
                Debug.Log("All questions have been asked.");
            }


        }

        int GetNextQuestionId()
        {
            if (_askedQuestions.Count >= _questions.Count)
            {
                return -1; // All questions have been asked
            }

            int questionId;
            do
            {
                questionId = Random.Range(0, _questions.Count);
            } while (_askedQuestions.Contains(questionId));

            _askedQuestions.Add(questionId);
            return questionId;
        }

        void DisplayQuestion(int questionId)
        {
            Question question = _questions[questionId];
            _correctAnswerIndex = question.correctAnswerIndex; //Store the correct answer index
            ManagerUI.Instance.UpdateQuestion(question);

            //Position the rings randomly
            PositionAnswerRings(question.answers);
        }

        void PositionAnswerRings(string[] answers)
        {
            Vector3 playerPosition = _playerController.transform.position;
            //float startXPosition = playerPosition.x - _ringSpacing * (answers.Length - 1) / 2; // Center the rings

            for (int i = 0; i < answers.Length; i++)
            {
                float ringYOffset = (i == 1) ? _middleRingYOffset : 0.0f; // Raise the middle ring
                float ringXOffset = (i - (answers.Length - 1) / 2.0f) * _ringSpacing; // Center the rings

                Vector3 ringPosition = new Vector3(playerPosition.x + ringXOffset, playerPosition.y + ringYOffset, playerPosition.z + _ringSpawnDistance);
                //Vector3 ringPosition = new Vector3(startXPosition + i * _ringSpacing, playerPosition.y + ringYOffset, playerPosition.z + _ringSpawnDistance);

                GameObject ring = Instantiate(answerRings[Random.Range(0, answerRings.Length)], ringPosition, Quaternion.Euler(90, 0, 0));
                ring.transform.SetParent(_levelContainer, true);

                TMP_Text answerText = ring.GetComponentInChildren<TMP_Text>();
                if (answerText != null && i < answers.Length)
                {
                    answerText.text = answers[i];
                }

                AnswerRing answerRing = ring.AddComponent<AnswerRing>();
                answerRing.AnswerIndex = i;
                answerRing.IsCorrect = i == _correctAnswerIndex;

                // Assign a unique name based on the answer text
                ring.name = "AnswerRing_" + answers[i];

            }
        }

        public void CheckAnswer(int answerIndex)
        {
            if (answerIndex == _correctAnswerIndex)
            {
                Debug.Log("Correct answer!");

            }
            else
            {
                Debug.Log("Wrong answer!");
            }

        }
    }
}