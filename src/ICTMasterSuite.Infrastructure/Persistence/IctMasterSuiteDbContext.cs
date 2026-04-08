using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence;

public sealed class IctMasterSuiteDbContext : DbContext, IAppDbContext
{
    public IctMasterSuiteDbContext(DbContextOptions<IctMasterSuiteDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<TechnicalAnalysis> TechnicalAnalyses => Set<TechnicalAnalysis>();
    public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => Set<KnowledgeBaseArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(600).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(400).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => new { x.Module, x.Action }).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            entity.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId);
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("UserSessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SessionToken).HasMaxLength(200).IsRequired();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<TechnicalAnalysis>(entity =>
        {
            entity.ToTable("TechnicalAnalyses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SerialNumber).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Model).HasMaxLength(120).IsRequired();
            entity.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.FilePath).HasMaxLength(1024).IsRequired();
            entity.Property(x => x.LogType).HasMaxLength(40).IsRequired();
            entity.Property(x => x.ErrorCode).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ErrorDescription).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Summary).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.TechnicianName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.AnalysisText).HasMaxLength(4000).IsRequired();
            entity.HasIndex(x => x.SerialNumber);
        });

        modelBuilder.Entity<KnowledgeBaseArticle>(entity =>
        {
            entity.ToTable("KnowledgeBaseArticles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Model).HasMaxLength(120).IsRequired();
            entity.Property(x => x.TestPhase).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Symptom).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Solution).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Author).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => new { x.Model, x.TestPhase });
        });

        SeedData.Apply(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
