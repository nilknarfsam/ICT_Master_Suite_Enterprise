namespace ICTMasterSuite.Application.Users.Dtos;

public sealed record UserDto(
    Guid Id,
    string FullName,
    string Username,
    string Email,
    bool IsActive,
    Guid RoleId,
    string RoleName,
    bool MustChangePassword,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
