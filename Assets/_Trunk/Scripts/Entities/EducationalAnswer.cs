namespace FlightGame.Entities
{
    public class EducationalAnswer
    {
        public string AnswerKey { get; private set; }
        public bool IsCorrect { get; private set; }

        public EducationalAnswer(string answerKey, bool isCorrect)
        {
            AnswerKey = answerKey;
            IsCorrect = isCorrect;
        }
    }
}