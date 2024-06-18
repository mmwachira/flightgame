using System.Collections.Generic;
using FlightGame.Entities;
using FlightGame.Managers;
using FlightGame.Questions;
using I2.Loc;
using QuickEngine.Extensions;
using TMPro;
using UnityEngine;

namespace FlightGame.Tracks
{
    public class ObstacleQuestion : Obstacle
    {
        [SerializeField] private TextMeshPro _question;
        [SerializeField] private AnswerRing _ringOne;
        [SerializeField] private AnswerRing _ringTwo;
        [SerializeField] private AnswerRing _ringThree;


        readonly Quaternion _defaultRotQuaternion = Quaternion.Euler(0f, 90f, 0f);

        private EducationalQuestion _questionData;
        public EducationalQuestion QuestionData => _questionData;

        public override void Setup()
        {
            List<EducationalQuestion> availableQuestions = ManagerEducationalContent.Instance.GetAvailableQuestions();
            if (availableQuestions.Count == 0)
            {
                ManagerEducationalContent.Instance.ResetAllQuestions();
                availableQuestions = ManagerEducationalContent.Instance.GetAvailableQuestions();
            }
            _questionData = availableQuestions[Random.Range(0, availableQuestions.Count)];
            if (_questionData.Answers.NotNullOrEmpty())
            {

                //ManagerUI.Instance.UpdateQuestion();

                //ManagerUI.Instance.question.text = LocalizationManager.GetTranslation(_questionData.QuestionKey);



                _question.text = LocalizationManager.GetTranslation(_questionData.QuestionKey);
                _ringOne.Setup(0, _questionData.Answers[0].AnswerKey, _questionData.Answers[0].IsCorrect);
                if (_questionData.Answers.Count > 1)
                {
                    _ringTwo.Setup(1, _questionData.Answers[1].AnswerKey, _questionData.Answers[1].IsCorrect);
                    if (_questionData.Answers.Count > 2)
                    {
                        _ringThree.Setup(2, _questionData.Answers[2].AnswerKey, _questionData.Answers[2].IsCorrect);
                    }
                }
            }
            _questionData.AlreadyAsked = true;
        }

        public override void Spawn(TrackSegment segment, float t)
        {
            Vector3 parentScale = segment.transform.localScale;
            Quaternion desiredRotation = _defaultRotQuaternion;
            if (parentScale.x < 0f)
            {
                desiredRotation = _defaultRotQuaternion * Quaternion.Euler(0f, 180f, 0f);
            }

            // Instantiate the object
            segment.GetPointAt(t, out var position, out var rotation);
            Vector3 finalPos = position;
            finalPos.y = 3f;

            ObstacleQuestion instantiatedObject = Instantiate(this, finalPos, desiredRotation, segment.ContainerObstacles);
            instantiatedObject.Setup();
        }
    }
}