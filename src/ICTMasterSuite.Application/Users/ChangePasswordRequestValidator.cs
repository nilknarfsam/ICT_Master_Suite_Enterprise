using FluentValidation;
using ICTMasterSuite.Application.Users.Dtos;

namespace ICTMasterSuite.Application.Users;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).NotEqual(x => x.CurrentPassword);
    }
}
