using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Application.Abstractions.Infrastructure;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<TechnicalAnalysis> TechnicalAnalyses { get; }
    DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
