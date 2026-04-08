namespace ICTMasterSuite.Application.KnowledgeBase.Dtos;

public sealed record UpdateKnowledgeBaseArticleRequest(
    Guid ArticleId,
    string Model,
    string TestPhase,
    string Symptom,
    string Solution,
    string Author);
