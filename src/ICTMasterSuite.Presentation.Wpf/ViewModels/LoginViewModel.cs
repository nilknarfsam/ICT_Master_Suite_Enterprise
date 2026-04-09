using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Application.Users.Dtos;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class LoginViewModel(
    IAuthenticationService authenticationService,
    IUserManagementService userManagementService,
    AuthenticatedUserState authenticatedUserState,
    AppSessionState appSessionState) : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmNewPassword = string.Empty;
    [ObservableProperty] private bool rememberMe;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isMandatoryPasswordChangeStep;

    private Guid? _pendingMandatoryPasswordUserId;

    public event EventHandler? LoginSucceeded;

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (IsBusy || IsMandatoryPasswordChangeStep)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        if (appSessionState.IsOffline)
        {
            ErrorMessage = "Sem conectividade com o banco/rede. O login online nao pode ser concluido neste momento.";
            IsBusy = false;
            return;
        }

        var result = await authenticationService.SignInAsync(new SignInRequest(Username, Password, RememberMe));
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessage = result.Message;
            IsBusy = false;
            return;
        }

        if (result.Value.MustChangePassword)
        {
            _pendingMandatoryPasswordUserId = result.Value.UserId;
            IsMandatoryPasswordChangeStep = true;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            ErrorMessage = "Defina uma nova senha antes de acessar o sistema (politica de bootstrap).";
            IsBusy = false;
            return;
        }

        authenticatedUserState.Set(result.Value);
        appSessionState.SetAuthentication(AuthenticationState.Authenticated);
        IsBusy = false;
        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task CompleteMandatoryPasswordChangeAsync()
    {
        if (IsBusy || !IsMandatoryPasswordChangeStep || _pendingMandatoryPasswordUserId is null)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmNewPassword)
        {
            ErrorMessage = "A confirmacao nao confere com a nova senha.";
            IsBusy = false;
            return;
        }

        var change = await userManagementService.ChangePasswordAsync(
            new ChangePasswordRequest(_pendingMandatoryPasswordUserId.Value, Password, NewPassword));

        if (!change.IsSuccess)
        {
            ErrorMessage = change.Message;
            IsBusy = false;
            return;
        }

        Password = NewPassword;
        var session = await authenticationService.SignInAsync(new SignInRequest(Username, NewPassword, RememberMe));
        if (!session.IsSuccess || session.Value is null)
        {
            ErrorMessage = session.Message;
            IsBusy = false;
            return;
        }

        if (session.Value.MustChangePassword)
        {
            ErrorMessage = "Ainda e necessario alterar a senha. Tente novamente ou contate o administrador.";
            IsBusy = false;
            return;
        }

        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
        IsMandatoryPasswordChangeStep = false;
        _pendingMandatoryPasswordUserId = null;

        authenticatedUserState.Set(session.Value);
        appSessionState.SetAuthentication(AuthenticationState.Authenticated);
        IsBusy = false;
        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
