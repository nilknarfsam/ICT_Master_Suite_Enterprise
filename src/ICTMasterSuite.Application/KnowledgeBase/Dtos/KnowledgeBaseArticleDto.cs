namespace ICTMasterSuite.Application.KnowledgeBase.Dtos;

public sealed record KnowledgeBaseArticleDto(
    Guid Id,
    string Model,
    string TestPhase,
    string Symptom,
    string Solution,
    string Author,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
