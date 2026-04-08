namespace ICTMasterSuite.Application.Users.Dtos;

public sealed record UpdateUserRequest(
    Guid UserId,
    string FullName,
    string Username,
    string Email,
    Guid RoleId);
