namespace ICTMasterSuite.Application.KnowledgeBase.Dtos;

public sealed record CreateKnowledgeBaseArticleRequest(
    string Model,
    string TestPhase,
    string Symptom,
    string Solution,
    string Author);
