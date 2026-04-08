using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Users.Dtos;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class UserManagementViewModel(IUserManagementService userManagementService) : ObservableObject
{
    public ObservableCollection<UserDto> Users { get; } = [];

    [ObservableProperty] private UserDto? selectedUser;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string fullNameInput = string.Empty;
    [ObservableProperty] private string usernameInput = string.Empty;
    [ObservableProperty] private string emailInput = string.Empty;
    [ObservableProperty] private string passwordInput = "ChangeMe@123";
    [ObservableProperty] private string operationMessage = string.Empty;

    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await RefreshAsync(SearchText);
    }

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        if (SelectedUser is null)
        {
            OperationMessage = "Selecione um usuario com role para basear o novo cadastro.";
            return;
        }

        var request = new CreateUserRequest(FullNameInput, UsernameInput, EmailInput, PasswordInput, SelectedUser.RoleId);
        var result = await userManagementService.CreateUserAsync(request);
        OperationMessage = result.IsSuccess ? "Usuario criado com sucesso." : result.Message;
        await RefreshAsync(SearchText);
    }

    [RelayCommand]
    private async Task UpdateSelectedAsync()
    {
        if (SelectedUser is null)
        {
            return;
        }

        var result = await userManagementService.UpdateUserAsync(new UpdateUserRequest(
            SelectedUser.Id,
            FullNameInput,
            UsernameInput,
            EmailInput,
            SelectedUser.RoleId));

        OperationMessage = result.IsSuccess ? "Usuario atualizado." : result.Message;
        await RefreshAsync(SearchText);
    }

    [RelayCommand]
    private async Task ToggleSelectedStatusAsync()
    {
        if (SelectedUser is null)
        {
            return;
        }

        var result = await userManagementService.SetUserActiveStatusAsync(SelectedUser.Id, !SelectedUser.IsActive);
        OperationMessage = result.IsSuccess ? "Status alterado." : result.Message;
        await RefreshAsync(SearchText);
    }

    partial void OnSelectedUserChanged(UserDto? value)
    {
        if (value is null)
        {
            return;
        }

        FullNameInput = value.FullName;
        UsernameInput = value.Username;
        EmailInput = value.Email;
    }

    private async Task RefreshAsync(string? query = null)
    {
        var users = await userManagementService.ListUsersAsync(query);
        Users.Clear();
        foreach (var user in users.Value ?? [])
        {
            Users.Add(user);
        }
    }
}
