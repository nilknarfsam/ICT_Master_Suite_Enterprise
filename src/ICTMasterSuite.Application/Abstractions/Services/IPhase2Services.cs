using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Users.Dtos;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<Result<SignInResponse>> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
    Task<Result> SignOutAsync(CancellationToken cancellationToken = default);
}

public interface IUserManagementService
{
    Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetUserActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<UserDto>>> ListUsersAsync(string? query, CancellationToken cancellationToken = default);
}

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userId, SystemModule module, PermissionAction action, CancellationToken cancellationToken = default);
}

public interface IAuditService
{
    Task<Result> RegisterAsync(string action, string details, CancellationToken cancellationToken = default);
}
