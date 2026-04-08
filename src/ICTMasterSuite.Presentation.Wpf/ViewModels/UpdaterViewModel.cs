using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Updater.UseCases;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class UpdaterViewModel(CheckForUpdatesUseCase useCase) : ObservableObject
{
    [ObservableProperty] private string currentVersion = "-";
    [ObservableProperty] private string latestVersion = "-";
    [ObservableProperty] private bool updateAvailable;
    [ObservableProperty] private string releaseNotes = string.Empty;
    [ObservableProperty] private string downloadUrl = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    [RelayCommand]
    private async Task CheckAsync()
    {
        var result = await useCase.ExecuteAsync();
        if (!result.IsSuccess || result.Value is null)
        {
            StatusMessage = result.Message;
            return;
        }

        CurrentVersion = result.Value.CurrentVersion;
        LatestVersion = result.Value.LatestVersion;
        UpdateAvailable = result.Value.UpdateAvailable;
        ReleaseNotes = result.Value.Notes;
        DownloadUrl = result.Value.DownloadUrl;
        StatusMessage = UpdateAvailable ? "Nova versão disponível." : "Aplicação atualizada.";
    }

    public Task CheckUpdatesAsync() => CheckAsync();
}
