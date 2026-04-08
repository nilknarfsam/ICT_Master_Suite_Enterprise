namespace ICTMasterSuite.Application.Authentication.Dtos;

public sealed record SignInRequest(string Username, string Password, bool RememberMe);
