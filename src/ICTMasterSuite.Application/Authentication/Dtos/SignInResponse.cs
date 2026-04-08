namespace ICTMasterSuite.Application.Authentication.Dtos;

public sealed record SignInResponse(
    Guid UserId,
    string FullName,
    string Username,
    string RoleName,
    string SessionToken,
    DateTime AuthenticatedAtUtc);
