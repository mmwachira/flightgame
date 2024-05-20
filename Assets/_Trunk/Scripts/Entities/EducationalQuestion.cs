using System.Collections.Generic;

public class EducationalQuestion
{
    public int CategoryNumber { get; private set; }
    public int Number { get; private set; }
    public string QuestionKey { get; private set; }
    public List<EducationalAnswer> Answers { get; private set; }

    public EducationalQuestion(int categoryNumber, int number, string questionKey)
    {
        CategoryNumber = categoryNumber;
        Number = number;
        QuestionKey = questionKey;
        Answers = new List<EducationalAnswer>();
    }

    public void AddAnswer(EducationalAnswer answer)
    {
        Answers.Add(answer);
    }
}