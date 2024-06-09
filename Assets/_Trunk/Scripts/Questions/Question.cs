[System.Serializable]
public class Question
{
    public int questionId;
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;

    public Question(int id, string text, string[] ans, int correctIndex)
    {
        questionId = id;
        questionText = text;
        answers = ans;
        correctAnswerIndex = correctIndex;
    }
}
