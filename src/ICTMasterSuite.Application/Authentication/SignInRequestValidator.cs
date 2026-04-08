using FluentValidation;
using ICTMasterSuite.Application.Authentication.Dtos;

namespace ICTMasterSuite.Application.Authentication;

public sealed class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
