namespace ICTMasterSuite.Application.Users.Dtos;

public sealed record CreateUserRequest(
    string FullName,
    string Username,
    string Email,
    string Password,
    Guid RoleId);
