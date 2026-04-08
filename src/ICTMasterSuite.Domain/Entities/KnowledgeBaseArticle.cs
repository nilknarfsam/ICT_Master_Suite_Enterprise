using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class KnowledgeBaseArticle : EntityBase
{
    public string Model { get; private set; }
    public string TestPhase { get; private set; }
    public string Symptom { get; private set; }
    public string Solution { get; private set; }
    public string Author { get; private set; }
    public bool IsActive { get; private set; }

    public KnowledgeBaseArticle(string model, string testPhase, string symptom, string solution, string author)
    {
        Model = model;
        TestPhase = testPhase;
        Symptom = symptom;
        Solution = solution;
        Author = author;
        IsActive = true;
    }

    public void Update(string model, string testPhase, string symptom, string solution, string author)
    {
        Model = model;
        TestPhase = testPhase;
        Symptom = symptom;
        Solution = solution;
        Author = author;
        Touch();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Touch();
    }
}
