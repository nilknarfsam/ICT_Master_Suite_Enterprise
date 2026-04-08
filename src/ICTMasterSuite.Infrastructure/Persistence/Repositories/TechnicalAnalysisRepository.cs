using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence.Repositories;

public sealed class TechnicalAnalysisRepository(IctMasterSuiteDbContext dbContext) : ITechnicalAnalysisRepository
{
    public Task AddAsync(TechnicalAnalysis analysis, CancellationToken cancellationToken = default)
    {
        return dbContext.TechnicalAnalyses.AddAsync(analysis, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<TechnicalAnalysis>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicalAnalyses
            .AsNoTracking()
            .Where(x => x.SerialNumber == serialNumber)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<TechnicalAnalysis?> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return dbContext.TechnicalAnalyses
            .AsNoTracking()
            .Where(x => x.SerialNumber == serialNumber)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TechnicalAnalysis>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicalAnalyses
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.TechnicalAnalyses.CountAsync(cancellationToken);
    }
}
