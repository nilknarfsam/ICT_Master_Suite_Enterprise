using System.Net;
using System.Reflection;
using System.Text.Json;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Updater.Dtos;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class UpdaterService(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IAuditLogger auditLogger) : IUpdaterService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public async Task<Result<VersionInfoDto>> CheckForUpdatesAsync(UpdaterCheckRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new UpdaterCheckRequest();
        var current = GetCurrentApplicationVersion();
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
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ICT-Master-Suite-Updater/1.0");

            using var response = await client.GetAsync(feedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Warning("Updater: HTTP {Status} ao obter feed {Url}", response.StatusCode, feedUrl);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var feed = await JsonSerializer.DeserializeAsync<RemoteUpdateFeedDto>(stream, JsonOptions, cancellationToken);
            if (feed is null || string.IsNullOrWhiteSpace(feed.LatestVersion))
            {
                Log.Warning("Updater: JSON inválido ou sem latestVersion em {Url}", feedUrl);
                return null;
            }

            var latest = feed.LatestVersion.Trim();
            var notes = string.IsNullOrWhiteSpace(feed.ReleaseNotes) ? "Sem notas de versão no feed." : feed.ReleaseNotes.Trim();
            var url = feed.DownloadUrl?.Trim() ?? string.Empty;
            var available = IsRemoteVersionNewer(currentVersion, latest);

            return new VersionInfoDto(
                currentVersion,
                latest,
                available,
                notes,
                url,
                UpdateCheckSource.RemoteFeed,
                feed.PublishedAt,
                feed.Integrity?.Sha256,
                feed.Integrity?.SignatureBase64);
        }
        catch (OperationCanceledException)
        {
            Log.Warning("Updater: operação cancelada ao acessar feed remoto.");
            return null;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Updater: falha ao consumir feed remoto (resiliente).");
            return null;
        }
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
        var available = IsRemoteVersionNewer(current, latest);
        return new VersionInfoDto(current, latest, available, displayNotes, downloadUrl, source, publishedAt, sha256, signature);
    }

    private static string AppendFallbackNotice(string baseNotes, string notice)
        => string.IsNullOrWhiteSpace(baseNotes) ? notice : $"{baseNotes}\n\n{notice}";

    private static bool IsRemoteVersionNewer(string current, string latest)
    {
        var a = NormalizeVersion(current);
        var b = NormalizeVersion(latest);
        if (Version.TryParse(a, out var v1) && Version.TryParse(b, out var v2))
        {
            return v2 > v1;
        }

        return !string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeVersion(string v)
    {
        var s = v.Trim().TrimStart('v', 'V');
        return s;
    }

    private static string GetCurrentApplicationVersion()
    {
        try
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            if (ver is null)
            {
                return "1.0.0";
            }

            return $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }
        catch
        {
            return "1.0.0";
        }
    }
}
