using ICTMasterSuite.Application.Authentication.Dtos;

namespace ICTMasterSuite.Presentation.Wpf.Services;

public sealed class AuthenticatedUserState
{
    public SignInResponse? Current { get; private set; }

    public bool IsAuthenticated => Current is not null;

    public void Set(SignInResponse user) => Current = user;

    public void Clear() => Current = null;
}
