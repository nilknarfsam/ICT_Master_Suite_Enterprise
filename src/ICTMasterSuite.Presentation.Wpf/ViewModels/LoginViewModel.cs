using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class LoginViewModel(
    IAuthenticationService authenticationService,
    AuthenticatedUserState authenticatedUserState) : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private bool rememberMe;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    public event EventHandler? LoginSucceeded;

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        var result = await authenticationService.SignInAsync(new SignInRequest(Username, Password, RememberMe));
        if (!result.IsSuccess || result.Value is null)
        {
            ErrorMessage = result.Message;
            IsBusy = false;
            return;
        }

        authenticatedUserState.Set(result.Value);
        IsBusy = false;
        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
