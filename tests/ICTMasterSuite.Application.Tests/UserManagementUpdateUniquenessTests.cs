using FluentAssertions;
using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Services;
using ICTMasterSuite.Application.Users;
using ICTMasterSuite.Application.Users.Dtos;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Application.Tests;

public class UserManagementUpdateUniquenessTests
{
    [Fact]
    public async Task UpdateUserAsync_WhenUsernameBelongsToAnotherUser_ReturnsBusinessError()
    {
        var role = new Role("Admin", "Admin");
        var roleId = role.Id;
        var other = new User("Other", "other.user", "other@local.test", "h", roleId);
        var subject = new User("Subject", "subject.user", "subject@local.test", "h", roleId);

        var sut = CreateSut([other, subject], role);

        var result = await sut.UpdateUserAsync(new UpdateUserRequest(subject.Id, subject.FullName, "other.user", subject.Email, roleId));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Username");
    }

    [Fact]
    public async Task UpdateUserAsync_WhenEmailBelongsToAnotherUser_ReturnsBusinessError()
    {
        var role = new Role("Admin", "Admin");
        var roleId = role.Id;
        var other = new User("Other", "other.user", "other@local.test", "h", roleId);
        var subject = new User("Subject", "subject.user", "subject@local.test", "h", roleId);

        var sut = CreateSut([other, subject], role);

        var result = await sut.UpdateUserAsync(new UpdateUserRequest(subject.Id, subject.FullName, subject.Username, "other@local.test", roleId));

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Email");
    }

    [Fact]
    public async Task UpdateUserAsync_WhenNoConflict_UpdatesSuccessfully()
    {
        var role = new Role("Admin", "Admin");
        var roleId = role.Id;
        var subject = new User("Subject", "subject.user", "subject@local.test", "h", roleId);

        var sut = CreateSut([subject], role);

        var result = await sut.UpdateUserAsync(new UpdateUserRequest(subject.Id, "New Name", "subject.user", "new@local.test", roleId));

        result.IsSuccess.Should().BeTrue();
        result.Value!.FullName.Should().Be("New Name");
        result.Value.Email.Should().Be("new@local.test");
    }

    private static UserManagementService CreateSut(IReadOnlyCollection<User> users, Role role)
    {
        var userRepo = new FakeUserRepository(users);
        var roleRepo = new FakeRoleRepository(role);
        return new UserManagementService(
            userRepo,
            roleRepo,
            new FakePasswordHasher(),
            new FakeAppDbContext(),
            new FakeAuditLogger(),
            new CreateUserRequestValidator(),
            new UpdateUserRequestValidator(),
            new ChangePasswordRequestValidator());
    }

    private sealed class FakeUserRepository(IReadOnlyCollection<User> users) : IUserRepository
    {
        private readonly List<User> _users = users.ToList();

        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));

        public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(u => u.Username == username));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

        public Task<IReadOnlyCollection<User>> ListAsync(string? query = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    private sealed class FakeRoleRepository(Role role) : IRoleRepository
    {
        public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
            => Task.FromResult<Role?>(roleId == role.Id ? role : null);

        public Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<bool> RoleHasPermissionAsync(Guid roleId, SystemModule module, PermissionAction action, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string plainTextPassword) => "x";
        public bool Verify(string plainTextPassword, string hash) => true;
    }

    private sealed class FakeAppDbContext : IAppDbContext
    {
        public DbSet<User> Users => default!;
        public DbSet<Role> Roles => default!;
        public DbSet<Permission> Permissions => default!;
        public DbSet<RolePermission> RolePermissions => default!;
        public DbSet<UserSession> UserSessions => default!;
        public DbSet<TechnicalAnalysis> TechnicalAnalyses => default!;
        public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => default!;
        public DbSet<SystemSetting> SystemSettings => default!;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class FakeAuditLogger : IAuditLogger
    {
        public Task<Result> WriteAsync(string action, string details, CancellationToken cancellationToken = default, string? module = null, string? user = null)
            => Task.FromResult(Result.Success());
    }
}
