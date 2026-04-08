using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Abstractions.Persistence;

public interface ITechnicalAnalysisRepository
{
    Task AddAsync(TechnicalAnalysis analysis, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TechnicalAnalysis>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<TechnicalAnalysis?> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TechnicalAnalysis>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
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
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
}

public interface ISystemSettingRepository
{
    Task<IReadOnlyCollection<SystemSetting>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<SystemSetting?> GetAsync(string category, string key, CancellationToken cancellationToken = default);
    Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default);
}
