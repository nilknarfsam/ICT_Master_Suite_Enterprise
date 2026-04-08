using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Abstractions.Updater;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Updater;
using ICTMasterSuite.Application.Updater.Dtos;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class UpdaterService(
    IConfiguration configuration,
    IUpdateFeedClient updateFeedClient,
    ICurrentApplicationVersionProvider currentVersionProvider,
    IAuditLogger auditLogger) : IUpdaterService
{
    public async Task<Result<VersionInfoDto>> CheckForUpdatesAsync(UpdaterCheckRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new UpdaterCheckRequest();
        var current = currentVersionProvider.GetCurrentVersion();
        var localLatest = configuration["Updater:LatestVersion"] ?? current;
        var localNotes = configuration["Updater:Notes"] ?? "Nenhuma nota configurada localmente.";
        var localUrl = configuration["Updater:DownloadUrl"] ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(request.RemoteFeedUrl))
        {
            var remote = await TryFetchRemoteFeedAsync(request.RemoteFeedUrl.Trim(), current, cancellationToken);
            if (remote is not null)
            {
                await auditLogger.WriteAsync(
                    "updater.check",
                    $"Source=Remote; Current={current}; Latest={remote.LatestVersion}; Available={remote.UpdateAvailable}",
                    cancellationToken);
                Log.Information(
                    "Updater: feed remoto OK. Current={Current} Latest={Latest} Source={Source}",
                    current,
                    remote.LatestVersion,
                    UpdateCheckSource.RemoteFeed);
                return Result<VersionInfoDto>.Success(remote);
            }

            var fallbackDto = BuildVersionInfo(
                current,
                localLatest,
                localNotes,
                localUrl,
                UpdateCheckSource.RemoteUnavailableFallback,
                null,
                null,
                null,
                AppendFallbackNotice(localNotes, "Feed remoto indisponível; exibindo referência local configurada."));

            await auditLogger.WriteAsync(
                "updater.check",
                $"Source=Fallback; Current={current}; Latest={fallbackDto.LatestVersion}",
                cancellationToken);
            Log.Warning("Updater: fallback para configuração local após falha no feed remoto.");
            return Result<VersionInfoDto>.Success(fallbackDto);
        }

        var localOnly = BuildVersionInfo(
            current,
            localLatest,
            localNotes,
            localUrl,
            UpdateCheckSource.LocalConfiguration,
            null,
            null,
            null,
            localNotes);

        await auditLogger.WriteAsync(
            "updater.check",
            $"Source=Local; Current={current}; Latest={localOnly.LatestVersion}; Available={localOnly.UpdateAvailable}",
            cancellationToken);
        Log.Information("Updater: verificação via configuração local. Current={Current} Latest={Latest}", current, localOnly.LatestVersion);
        return Result<VersionInfoDto>.Success(localOnly);
    }

    private async Task<VersionInfoDto?> TryFetchRemoteFeedAsync(string feedUrl, string currentVersion, CancellationToken cancellationToken)
    {
        var snapshot = await updateFeedClient.FetchAsync(feedUrl, cancellationToken);
        if (snapshot is null)
        {
            return null;
        }

        var notes = string.IsNullOrWhiteSpace(snapshot.ReleaseNotes) ? "Sem notas de versão no feed." : snapshot.ReleaseNotes.Trim();
        var url = snapshot.DownloadUrl?.Trim() ?? string.Empty;
        var available = UpdateVersionComparer.IsRemoteNewer(currentVersion, snapshot.LatestVersion);

        return new VersionInfoDto(
            currentVersion,
            snapshot.LatestVersion,
            available,
            notes,
            url,
            UpdateCheckSource.RemoteFeed,
            snapshot.PublishedAt,
            snapshot.IntegritySha256,
            snapshot.IntegritySignatureBase64);
    }

    private static VersionInfoDto BuildVersionInfo(
        string current,
        string latest,
        string notes,
        string downloadUrl,
        UpdateCheckSource source,
        DateTimeOffset? publishedAt,
        string? sha256,
        string? signature,
        string displayNotes)
    {
        var available = UpdateVersionComparer.IsRemoteNewer(current, latest);
        return new VersionInfoDto(current, latest, available, displayNotes, downloadUrl, source, publishedAt, sha256, signature);
    }

    private static string AppendFallbackNotice(string baseNotes, string notice)
        => string.IsNullOrWhiteSpace(baseNotes) ? notice : $"{baseNotes}\n\n{notice}";
}
