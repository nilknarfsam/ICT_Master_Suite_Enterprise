using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;

namespace ICTMasterSuite.Application.KnowledgeBase.UseCases;

public sealed class CreateKnowledgeBaseArticleUseCase(IKnowledgeBaseService knowledgeBaseService)
{
    public Task<Result<KnowledgeBaseArticleDto>> ExecuteAsync(CreateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default)
        => knowledgeBaseService.CreateAsync(request, cancellationToken);
}
