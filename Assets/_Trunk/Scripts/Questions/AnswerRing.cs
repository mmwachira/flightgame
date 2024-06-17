using I2.Loc;
using TMPro;
using UnityEngine;

namespace FlightGame.Questions
{
    public class AnswerRing : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _answerText;
        
        public int AnswerIndex { get; private set; }
        public bool IsCorrect { get; private set; }

        public void Setup(int index, string text, bool isCorrect)
        {
            gameObject.SetActive(true);
            AnswerIndex = index;
            _answerText.text = LocalizationManager.GetTranslation(text);
            IsCorrect = isCorrect;
        }
    }
}