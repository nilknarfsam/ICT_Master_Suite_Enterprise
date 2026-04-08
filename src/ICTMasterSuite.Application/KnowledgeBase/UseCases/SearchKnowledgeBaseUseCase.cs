using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;

namespace ICTMasterSuite.Application.KnowledgeBase.UseCases;

public sealed class SearchKnowledgeBaseUseCase(IKnowledgeBaseService knowledgeBaseService)
{
    public Task<Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>> ExecuteAsync(SearchKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
        => knowledgeBaseService.SearchAsync(request, cancellationToken);
}
