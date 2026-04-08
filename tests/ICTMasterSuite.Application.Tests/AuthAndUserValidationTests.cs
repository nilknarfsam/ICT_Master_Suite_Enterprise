using ICTMasterSuite.Application.Authentication;
using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Application.Users;
using ICTMasterSuite.Application.Users.Dtos;

namespace ICTMasterSuite.Application.Tests;

public class AuthAndUserValidationTests
{
    [Fact]
    public void SignInRequestValidator_ShouldRejectInvalidPayload()
    {
        var validator = new SignInRequestValidator();
        var result = validator.Validate(new SignInRequest("", "123", false));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateUserRequestValidator_ShouldAcceptValidPayload()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest("Test User", "test.user", "test.user@ict.local", "Strong@123", Guid.NewGuid());

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
