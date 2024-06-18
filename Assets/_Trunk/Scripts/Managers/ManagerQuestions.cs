using System.Collections.Generic;
using FlightGame.Players;
using FlightGame.Questions;
using FlightGame.Tracks;
using TMPro;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerQuestions : MonoBehaviour
    {
        public static ManagerQuestions Instance { get; private set; }

        public string[] questions => _question;
        public string[] answers => _answer;

        [SerializeField] private GameObject[] answerRings;
        [SerializeField] private float _ringSpawnDistance = 10f; // Distance ahead of the player to spawn rings
        [SerializeField] private float _ringSpacing = 0.5f; // Spacing between rings
        [SerializeField] private float _laneOffset = 1.4f;
        private string[] _answer;
        private string[] _question;
        private List<Question> _questions;
        private HashSet<int> _askedQuestions;
        private Vector3[] ringPositions;
        private int _currentQuestionId;
        private int _correctAnswerIndex;

        private PlayerController _playerController;
        private GameObject ringPrefab;

        private const int ObstacleLayer = 6; // Change it according to layer setup
        private static readonly int ObstacleLayerMask = 1 << ObstacleLayer;


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

            _questions = new List<Question>
       {
        new Question(0, "What color is the plane?", new string[]{"Red", "Blue", "Green" }, 0),
        new Question(1, "When you make a new friend, how do you feel?", new string[] { "Happy", "Sad", "Nervous" }, 0),
        new Question(2, "When you cry, how do you feel?", new string[] { "Sad", "Relieved", "Comforted" }, 0)
       };

            _askedQuestions = new HashSet<int>();
            _currentQuestionId = -1;

            ringPositions = new Vector3[]
                    {
                        new Vector3(0, 1.89f, 0.02f), // Central lane
                        new Vector3(0, 0, 2.2f),  // Left lane
                        new Vector3(0, 0, -2.2f),  // Right lane
                        //new Vector3(0, -1.89f, 0) //Bottom position
                    };


        }

        public void AskQuestion(TrackSegment segment)
        {

            _currentQuestionId = GetNextQuestionId();
            if (_currentQuestionId != -1)
            {
                DisplayQuestion(segment, _currentQuestionId);
                ShuffleRings();
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

        void DisplayQuestion(TrackSegment segment, int questionId)
        {
            Question question = _questions[questionId];
            _correctAnswerIndex = question.correctAnswerIndex; //Store the correct answer index
            //ManagerUI.Instance.UpdateQuestion(question);

            //Position the rings randomly
            PositionAnswerRings(segment, question.answers);
        }

        void PositionAnswerRings(TrackSegment segment, string[] answers)
        {
            float currentWorldPosition = 0.0f;

            Vector3 playerPosition = _playerController.transform.position;

            segment.GetPointAtInWorldUnit(currentWorldPosition, out var segmentPosition, out var segmentRotation);

            Vector3 ringPosition = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z + _ringSpawnDistance);

            ringPrefab = Instantiate(answerRings[0], ringPosition, Quaternion.Euler(0, -90, 0));
            ringPrefab.transform.SetParent(segment.ContainerQuestions, true);

            Transform[] ringTransforms = ringPrefab.GetComponentsInChildren<Transform>();
            List<Transform> rings = new List<Transform>();
            List<TMP_Text> answerTexts = new List<TMP_Text>();

            foreach (var ring in ringTransforms)
            {
                if (ring.name.StartsWith("Ring"))
                {
                    rings.Add(ring);
                    var textComponent = ring.GetComponentInChildren<TMP_Text>();
                    if (textComponent != null)
                    {
                        answerTexts.Add(textComponent);
                    }
                }
            }

            if (answerTexts.Count != answers.Length || rings.Count != answers.Length)
            {
                Debug.LogError("Mismatch between number of answers and number of rings or text components in the prefab.");
                return;
            }

            int currentLane = Random.Range(0, 3);


            for (int i = 0; i < answers.Length; i++)
            {
                bool laneValid = true;
                int testedLane = currentLane;
                while (Physics.CheckSphere(segmentPosition + ((testedLane - 1) * _laneOffset * (segmentRotation * Vector3.right)), 0.4f, ObstacleLayerMask))
                {
                    testedLane = (testedLane + 1) % 3;
                    if (currentLane == testedLane)
                    {
                        // Couldn't find a valid lane.
                        laneValid = false;
                        break;
                    }
                }

                if (laneValid)
                {


                    Transform ring = rings[i];  //Skip the parent transform

                    ring.localPosition = ringPositions[i];

                    TMP_Text answerText = answerTexts[i];
                    answerText.text = answers[i];

                    AnswerRing answerRing = segment.ContainerQuestions.gameObject.AddComponent<AnswerRing>();
                    answerRing.Setup(i, answers[i], i == _correctAnswerIndex);
                    // answerRing.AnswerIndex = i;
                    // answerRing.IsCorrect = i == _correctAnswerIndex;


                }

                currentLane = (currentLane + 1) % 3;

            }


        }

        void ShuffleRings()
        {
            for (int i = 0; i < ringPositions.Length; i++)
            {
                Vector3 temp = ringPositions[i];
                int randomIndex = Random.Range(0, ringPositions.Length);
                ringPositions[i] = ringPositions[randomIndex];
                ringPositions[randomIndex] = temp;
            }

            // Update the positions of the rings
            for (int i = 0; i < ringPrefab.transform.childCount; i++)
            {
                Transform ring = ringPrefab.transform.GetChild(i);
                ring.localPosition = ringPositions[i];
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