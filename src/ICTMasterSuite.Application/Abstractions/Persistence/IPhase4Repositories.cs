using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Abstractions.Persistence;

public interface ITechnicalAnalysisRepository
{
    Task AddAsync(TechnicalAnalysis analysis, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TechnicalAnalysis>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<TechnicalAnalysis?> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
}

public interface IKnowledgeBaseArticleRepository
{
    Task AddAsync(KnowledgeBaseArticle article, CancellationToken cancellationToken = default);
    Task<KnowledgeBaseArticle?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<KnowledgeBaseArticle>> SearchAsync(
        string? model,
        string? testPhase,
        string? term,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
