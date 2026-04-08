using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence.Repositories;

public sealed class KnowledgeBaseArticleRepository(IctMasterSuiteDbContext dbContext) : IKnowledgeBaseArticleRepository
{
    public Task AddAsync(KnowledgeBaseArticle article, CancellationToken cancellationToken = default)
    {
        return dbContext.KnowledgeBaseArticles.AddAsync(article, cancellationToken).AsTask();
    }

    public Task<KnowledgeBaseArticle?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        return dbContext.KnowledgeBaseArticles.FirstOrDefaultAsync(x => x.Id == articleId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<KnowledgeBaseArticle>> SearchAsync(
        string? model,
        string? testPhase,
        string? term,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.KnowledgeBaseArticles.AsNoTracking().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            query = query.Where(x => x.Model.Contains(model));
        }

        if (!string.IsNullOrWhiteSpace(testPhase))
        {
            query = query.Where(x => x.TestPhase.Contains(testPhase));
        }

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(x => x.Symptom.Contains(term) || x.Solution.Contains(term));
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }
}
