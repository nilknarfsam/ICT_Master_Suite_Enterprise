namespace ICTMasterSuite.Application.KnowledgeBase.Dtos;

public sealed record SearchKnowledgeBaseRequest(string? Model, string? TestPhase, string? Term, bool IncludeInactive = false);
