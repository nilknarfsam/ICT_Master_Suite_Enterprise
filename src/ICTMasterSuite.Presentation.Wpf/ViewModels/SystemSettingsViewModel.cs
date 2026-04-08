using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Configuration.Dtos;
using ICTMasterSuite.Application.Configuration.UseCases;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class SystemSettingsViewModel(
    GetSystemConfigurationUseCase getSystemConfigurationUseCase,
    SaveSystemConfigurationUseCase saveSystemConfigurationUseCase) : ObservableObject
{
    [ObservableProperty] private string finderDirectories = string.Empty;
    [ObservableProperty] private string updaterEndpoint = string.Empty;
    [ObservableProperty] private bool autoCheckUpdates = true;
    [ObservableProperty] private string preferredTheme = "Dark";
    [ObservableProperty] private bool autoOpenLastModule = true;
    [ObservableProperty] private string statusMessage = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        var result = await getSystemConfigurationUseCase.ExecuteAsync();
        if (!result.IsSuccess || result.Value is null)
        {
            StatusMessage = result.Message;
            return;
        }

        FinderDirectories = result.Value.FinderDirectories;
        UpdaterEndpoint = result.Value.UpdaterEndpoint;
        AutoCheckUpdates = result.Value.AutoCheckUpdates;
        PreferredTheme = result.Value.PreferredTheme;
        AutoOpenLastModule = result.Value.AutoOpenLastModule;
        StatusMessage = "Configurações carregadas.";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var payload = new SystemConfigurationDto(
            FinderDirectories,
            UpdaterEndpoint,
            AutoCheckUpdates,
            PreferredTheme,
            AutoOpenLastModule);

        var result = await saveSystemConfigurationUseCase.ExecuteAsync(payload);
        StatusMessage = result.Message;
    }
}
