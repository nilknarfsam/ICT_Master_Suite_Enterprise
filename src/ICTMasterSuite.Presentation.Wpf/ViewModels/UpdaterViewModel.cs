using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Updater.Dtos;
using ICTMasterSuite.Application.Updater.UseCases;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class UpdaterViewModel(CheckForUpdatesUseCase useCase, ISystemConfigurationService configurationService) : ObservableObject
{
    [ObservableProperty] private string currentVersion = "-";
    [ObservableProperty] private string latestVersion = "-";
    [ObservableProperty] private bool updateAvailable;
    [ObservableProperty] private string releaseNotes = string.Empty;
    [ObservableProperty] private string downloadUrl = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string checkSourceDescription = string.Empty;
    [ObservableProperty] private string? remotePublishedDisplay;
    [ObservableProperty] private string? integrityPreview;
    [ObservableProperty] private string updateStatusLabel = string.Empty;

    [RelayCommand]
    private async Task CheckAsync()
    {
        var settings = await configurationService.GetAsync();
        var endpoint = settings.Value?.UpdaterEndpoint?.Trim();
        var request = new UpdaterCheckRequest(string.IsNullOrWhiteSpace(endpoint) ? null : endpoint);

        var result = await useCase.ExecuteAsync(request);
        if (!result.IsSuccess || result.Value is null)
        {
            StatusMessage = string.IsNullOrWhiteSpace(result.Message) ? "Não foi possível verificar atualizações." : result.Message;
            UpdateStatusLabel = string.Empty;
            return;
        }

        CurrentVersion = result.Value.CurrentVersion;
        LatestVersion = result.Value.LatestVersion;
        UpdateAvailable = result.Value.UpdateAvailable;
        ReleaseNotes = result.Value.Notes;
        DownloadUrl = result.Value.DownloadUrl;
        CheckSourceDescription = DescribeSource(result.Value.Source);
        RemotePublishedDisplay = result.Value.RemotePublishedAt?.ToString("yyyy-MM-dd HH:mm 'UTC'zzz");
        IntegrityPreview = BuildIntegrityPreview(result.Value.IntegritySha256, result.Value.IntegritySignatureBase64);
        UpdateStatusLabel = result.Value.UpdateAvailable
            ? "Nova versão disponível em relação à referência."
            : "Nenhuma atualização obrigatória em relação à referência consultada.";

        StatusMessage = result.Value.Source == UpdateCheckSource.RemoteUnavailableFallback
            ? "Verificação concluída (feed remoto indisponível — usando referência local)."
            : UpdateAvailable
                ? "Nova versão disponível."
                : "Aplicação atualizada em relação à referência consultada.";
    }

    public Task CheckUpdatesAsync() => CheckAsync();

    private static string DescribeSource(UpdateCheckSource source) => source switch
    {
        UpdateCheckSource.RemoteFeed => "Fonte: feed remoto JSON",
        UpdateCheckSource.LocalConfiguration => "Fonte: configuração local (sem URL de feed configurada)",
        UpdateCheckSource.RemoteUnavailableFallback => "Fonte: fallback local (após falha no feed remoto)",
        _ => string.Empty
    };

    private static string? BuildIntegrityPreview(string? sha256, string? signatureB64)
    {
        if (!string.IsNullOrWhiteSpace(sha256))
        {
            var s = sha256.Trim();
            return s.Length > 24 ? $"SHA-256: {s[..12]}…{s[^8..]}" : $"SHA-256: {s}";
        }

        if (!string.IsNullOrWhiteSpace(signatureB64))
        {
            return "Assinatura: presente (validação futura)";
        }

        return null;
    }
}
