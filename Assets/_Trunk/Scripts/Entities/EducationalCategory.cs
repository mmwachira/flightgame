using System.Collections.Generic;

public class EducationalCategory
{
    public int CategoryNumber { get; private set; }
    public string CategoryKey { get; private set; }
    public List<EducationalQuestion> Questions { get; private set; }

    public EducationalCategory(int categoryNumber, string categoryKey)
    {
        CategoryNumber = categoryNumber;
        CategoryKey = categoryKey;
        Questions = new List<EducationalQuestion>();
    }

    public void AddQuestion(EducationalQuestion question)
    {
        Questions.Add(question);
    }
}