namespace ICTMasterSuite.Application.Updater.Dtos;

/// <summary>Requisição de verificação de versão (feed remoto opcional).</summary>
public sealed record UpdaterCheckRequest(string? RemoteFeedUrl = null);

public enum UpdateCheckSource
{
    RemoteFeed = 1,
    LocalConfiguration = 2,
    RemoteUnavailableFallback = 3
}

/// <summary>Informações de versão para o módulo Updater (preparado para integridade futura).</summary>
public sealed record VersionInfoDto(
    string CurrentVersion,
    string LatestVersion,
    bool UpdateAvailable,
    string Notes,
    string DownloadUrl,
    UpdateCheckSource Source,
    DateTimeOffset? RemotePublishedAt,
    string? IntegritySha256,
    string? IntegritySignatureBase64);
