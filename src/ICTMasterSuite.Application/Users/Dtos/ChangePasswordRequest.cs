namespace ICTMasterSuite.Application.Users.Dtos;

public sealed record ChangePasswordRequest(Guid UserId, string CurrentPassword, string NewPassword);
