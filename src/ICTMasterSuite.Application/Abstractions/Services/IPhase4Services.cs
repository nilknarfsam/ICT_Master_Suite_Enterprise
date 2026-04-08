using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;

namespace ICTMasterSuite.Application.Abstractions.Services;

public interface ITechnicalHistoryService
{
    Task<Result<TechnicalAnalysisDto>> SaveTechnicalAnalysisAsync(SaveTechnicalAnalysisRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<TechnicalAnalysisDto>>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<Result<TechnicalAnalysisDto>> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default);
}

public interface IKnowledgeBaseService
{
    Task<Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>> SearchAsync(SearchKnowledgeBaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<KnowledgeBaseArticleDto>> CreateAsync(CreateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default);
    Task<Result<KnowledgeBaseArticleDto>> UpdateAsync(UpdateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetActiveStatusAsync(Guid articleId, bool isActive, CancellationToken cancellationToken = default);
}
