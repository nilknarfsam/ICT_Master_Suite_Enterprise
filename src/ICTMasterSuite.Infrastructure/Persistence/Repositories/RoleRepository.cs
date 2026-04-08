using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(IctMasterSuiteDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return dbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
    }

    public Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);
    }

    public Task<bool> RoleHasPermissionAsync(Guid roleId, SystemModule module, PermissionAction action, CancellationToken cancellationToken = default)
    {
        return dbContext.RolePermissions
            .Include(x => x.Permission)
            .AnyAsync(x => x.RoleId == roleId && x.Permission.Module == module && x.Permission.Action == action, cancellationToken);
    }
}
