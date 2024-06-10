using System.Collections.Generic;
using FlightGame.Entities;
using I2.Loc;
using UnityEngine;

namespace FlightGame.Managers
{
    public class ManagerEducationalContent : MonoBehaviour
    {
        public static ManagerEducationalContent Instance { get; private set; }
    
        private readonly List<EducationalCategory> _questionCategories = new List<EducationalCategory>();
        private const string FlightGameKey = "FlightGame/question_";
    
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
    
        public void LoadEducationalQuestions()
        {
            _questionCategories.Clear();

            int indexCategory = 0;
            bool validCategory = true;
            while (validCategory)
            {
                indexCategory++;
                string key = FlightGameKey + indexCategory;
                string localisation = LocalizationManager.GetTranslation(key);

                validCategory = !string.IsNullOrEmpty(localisation);
                if (validCategory)
                {
                    EducationalCategory category = new EducationalCategory(indexCategory, key);

                    int indexQuestion = 0;
                    bool validQuestion = true;
                    while (validQuestion)
                    {
                        indexQuestion++;
                        key = FlightGameKey + indexCategory + "_" + indexQuestion;
                        localisation = LocalizationManager.GetTranslation(key);

                        validQuestion = !string.IsNullOrEmpty(localisation);
                        if (validQuestion)
                        {
                            EducationalQuestion question = new EducationalQuestion(indexCategory, indexQuestion, key);

                            int indexAnswer = 0;
                            bool validAnswer = true;
                            while (validAnswer)
                            {
                                indexAnswer++;
                                string answerKey = key + "_" + indexAnswer;
                                string answerLocalisation = LocalizationManager.GetTranslation(answerKey);
                                string correctAnswerKey = answerKey + "_correct";
                                string correctAnswerLocalisation = LocalizationManager.GetTranslation(correctAnswerKey);

                                validAnswer = !string.IsNullOrEmpty(answerLocalisation) || !string.IsNullOrEmpty(correctAnswerLocalisation);
                                if (validAnswer)
                                {
                                    bool isCorrect = !string.IsNullOrEmpty(correctAnswerLocalisation);
                                    string answerKeyToUse = isCorrect ? correctAnswerKey : answerKey;
                                    EducationalAnswer answer = new EducationalAnswer(answerKeyToUse, isCorrect);
                                    question.AddAnswer(answer);
                                }
                            }

                            category.AddQuestion(question);
                        }
                    }

                    _questionCategories.Add(category);
                }
            }
        }
    }
}
