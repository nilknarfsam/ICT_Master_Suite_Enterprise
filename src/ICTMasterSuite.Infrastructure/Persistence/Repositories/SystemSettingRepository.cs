using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence.Repositories;

public sealed class SystemSettingRepository(IctMasterSuiteDbContext dbContext) : ISystemSettingRepository
{
    public async Task<IReadOnlyCollection<SystemSetting>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SystemSettings.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<SystemSetting?> GetAsync(string category, string key, CancellationToken cancellationToken = default)
    {
        return dbContext.SystemSettings.FirstOrDefaultAsync(x => x.Category == category && x.Key == key, cancellationToken);
    }

    public Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        return dbContext.SystemSettings.AddAsync(setting, cancellationToken).AsTask();
    }
}
