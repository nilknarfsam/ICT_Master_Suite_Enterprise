using FluentValidation;
using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Users.Dtos;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Application.Services;

public sealed class AuthenticationService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ISessionStore sessionStore,
    IAppDbContext dbContext,
    IAuditLogger auditLogger,
    IValidator<SignInRequest> validator) : IAuthenticationService
{
    public async Task<Result<SignInResponse>> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default)
    {
        await auditLogger.WriteAsync("auth.signin.attempt", $"Username: {request.Username}", cancellationToken);

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            await auditLogger.WriteAsync("auth.signin.validation.failed", $"Username: {request.Username}", cancellationToken);
            return Result<SignInResponse>.Failure(validation.ToString());
        }

        User? user;
        try
        {
            user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        }
        catch (Exception ex)
        {
            await auditLogger.WriteAsync("auth.signin.db.failure", $"Username: {request.Username}; Error: {ex.GetType().Name}", cancellationToken);
            return Result<SignInResponse>.Failure("Falha de conexao com a base de autenticacao.");
        }

        if (user is null)
        {
            await auditLogger.WriteAsync("auth.signin.user.notfound", $"Username: {request.Username}", cancellationToken);
            return Result<SignInResponse>.Failure("Credenciais invalidas.");
        }

        if (!user.IsActive)
        {
            await auditLogger.WriteAsync("auth.signin.user.inactive", $"UserId: {user.Id}", cancellationToken);
            return Result<SignInResponse>.Failure("Usuario inativo.");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            await auditLogger.WriteAsync("auth.signin.password.invalid", $"UserId: {user.Id}", cancellationToken);
            return Result<SignInResponse>.Failure("Credenciais invalidas.");
        }

        var session = new UserSession(user.Id, Guid.NewGuid().ToString("N"));
        await sessionStore.SetCurrentSessionAsync(session, cancellationToken);
        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new SignInResponse(user.Id, user.FullName, user.Username, user.Role?.Name ?? "N/A", session.SessionToken, DateTime.UtcNow);
        await auditLogger.WriteAsync("auth.signin.success", $"UserId: {user.Id}", cancellationToken);
        return Result<SignInResponse>.Success(response);
    }

    public async Task<Result> SignOutAsync(CancellationToken cancellationToken = default)
    {
        var current = await sessionStore.GetCurrentSessionAsync(cancellationToken);
        if (current is null)
        {
            return Result.Success();
        }

        var dbSession = await dbContext.UserSessions.FindAsync([current.Id], cancellationToken);
        dbSession?.End();
        await dbContext.SaveChangesAsync(cancellationToken);
        await sessionStore.ClearCurrentSessionAsync(cancellationToken);
        await auditLogger.WriteAsync("auth.signout", $"UserId: {current.UserId}", cancellationToken);
        return Result.Success();
    }
}

public sealed class UserManagementService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IAppDbContext dbContext,
    IAuditLogger auditLogger,
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator) : IUserManagementService
{
    public async Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Failure(validation.ToString());
        }

        if (await userRepository.GetByUsernameAsync(request.Username, cancellationToken) is not null)
        {
            return Result<UserDto>.Failure("Username ja existe.");
        }

        if (await userRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            return Result<UserDto>.Failure("Email ja existe.");
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Result<UserDto>.Failure("Perfil nao encontrado.");
        }

        var user = new User(request.FullName, request.Username, request.Email, passwordHasher.Hash(request.Password), request.RoleId);
        await userRepository.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("user.create", $"UserId: {user.Id}", cancellationToken);

        return Result<UserDto>.Success(MapUser(user, role.Name));
    }

    public async Task<Result<UserDto>> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Failure(validation.ToString());
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (user is null || role is null)
        {
            return Result<UserDto>.Failure("Usuario ou perfil invalido.");
        }

        user.UpdateProfile(request.FullName, request.Username, request.Email, request.RoleId);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("user.update", $"UserId: {user.Id}", cancellationToken);
        return Result<UserDto>.Success(MapUser(user, role.Name));
    }

    public async Task<Result> SetUserActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Usuario nao encontrado.");
        }

        if (isActive) user.Activate();
        else user.Deactivate();

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("user.status", $"UserId: {user.Id}; Active: {isActive}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Usuario nao encontrado.");
        }

        user.ChangePassword(passwordHasher.Hash(newPassword));
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("user.password.reset", $"UserId: {user.Id}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await changePasswordValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToString());
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure("Senha atual invalida.");
        }

        user.ChangePassword(passwordHasher.Hash(request.NewPassword));
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("user.password.change", $"UserId: {user.Id}", cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<UserDto>>> ListUsersAsync(string? query, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.ListAsync(query, cancellationToken);
        var result = users.Select(user => MapUser(user, user.Role?.Name ?? "N/A")).ToList();
        return Result<IReadOnlyCollection<UserDto>>.Success(result);
    }

    private static UserDto MapUser(User user, string roleName)
        => new(user.Id, user.FullName, user.Username, user.Email, user.IsActive, user.RoleId, roleName, user.CreatedAt, user.UpdatedAt);
}

public sealed class AuthorizationService(IUserRepository userRepository, IRoleRepository roleRepository) : IAuthorizationService
{
    public async Task<bool> HasPermissionAsync(Guid userId, SystemModule module, PermissionAction action, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return false;
        }

        return await roleRepository.RoleHasPermissionAsync(user.RoleId, module, action, cancellationToken);
    }
}
