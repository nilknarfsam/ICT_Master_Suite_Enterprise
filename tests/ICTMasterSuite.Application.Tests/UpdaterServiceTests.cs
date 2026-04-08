using FluentAssertions;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Updater;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Updater;
using ICTMasterSuite.Application.Updater.Dtos;
using ICTMasterSuite.Infrastructure.Services;

namespace ICTMasterSuite.Application.Tests;

public class UpdaterServiceTests
{
    [Fact]
    public async Task RemoteFeed_Success_ReturnsRemoteSource()
    {
        var snapshot = new RemoteUpdateFeedSnapshot("1.2.0", "n", "https://dl", null, null, null);
        var sut = CreateSut(
            new StubFeedClient(snapshot),
            new FixedVersionProvider("1.0.0"),
            new Dictionary<string, string?>
            {
                ["Updater:LatestVersion"] = "9.9.9",
                ["Updater:Notes"] = "local",
                ["Updater:DownloadUrl"] = ""
            });

        var result = await sut.CheckForUpdatesAsync(new UpdaterCheckRequest("https://feed.example/app.json"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Source.Should().Be(UpdateCheckSource.RemoteFeed);
        result.Value.LatestVersion.Should().Be("1.2.0");
        result.Value.UpdateAvailable.Should().BeTrue();
        result.Value.Notes.Should().Be("n");
        result.Value.DownloadUrl.Should().Be("https://dl");
    }

    [Fact]
    public async Task RemoteFeed_HttpFails_UsesFallbackFromConfiguration()
    {
        var sut = CreateSut(
            new StubFeedClient(null),
            new FixedVersionProvider("1.0.0"),
            new Dictionary<string, string?>
            {
                ["Updater:LatestVersion"] = "1.5.0",
                ["Updater:Notes"] = "nota local",
                ["Updater:DownloadUrl"] = "https://offline"
            });

        var result = await sut.CheckForUpdatesAsync(new UpdaterCheckRequest("https://feed.example/x.json"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Source.Should().Be(UpdateCheckSource.RemoteUnavailableFallback);
        result.Value.LatestVersion.Should().Be("1.5.0");
        result.Value.Notes.Should().Contain("nota local");
        result.Value.Notes.Should().Contain("Feed remoto indisponível");
        result.Value.DownloadUrl.Should().Be("https://offline");
    }

    [Fact]
    public async Task NoRemoteUrl_UsesLocalConfigurationOnly()
    {
        var sut = CreateSut(
            new StubFeedClient(null),
            new FixedVersionProvider("1.0.0"),
            new Dictionary<string, string?>
            {
                ["Updater:LatestVersion"] = "1.0.0",
                ["Updater:Notes"] = "dev",
                ["Updater:DownloadUrl"] = ""
            });

        var result = await sut.CheckForUpdatesAsync(new UpdaterCheckRequest(null));

        result.IsSuccess.Should().BeTrue();
        result.Value!.Source.Should().Be(UpdateCheckSource.LocalConfiguration);
        result.Value.LatestVersion.Should().Be("1.0.0");
        result.Value.Notes.Should().Be("dev");
    }

    [Fact]
    public async Task RemoteFeed_OptionalReleaseNotes_UsesDefaultMessage()
    {
        var snapshot = new RemoteUpdateFeedSnapshot("1.0.1", null, null, null, null, null);
        var sut = CreateSut(
            new StubFeedClient(snapshot),
            new FixedVersionProvider("1.0.0"),
            ConfigMinimal());

        var result = await sut.CheckForUpdatesAsync(new UpdaterCheckRequest("https://feed/x.json"));

        result.Value!.Notes.Should().Be("Sem notas de versão no feed.");
    }

    private static FlatConfiguration BuildConfig(IReadOnlyDictionary<string, string?> pairs) => new(pairs);

    private static Dictionary<string, string?> ConfigMinimal() => new()
    {
        ["Updater:LatestVersion"] = "1.0.0",
        ["Updater:Notes"] = "x",
        ["Updater:DownloadUrl"] = ""
    };

    private static UpdaterService CreateSut(
        IUpdateFeedClient feedClient,
        ICurrentApplicationVersionProvider versionProvider,
        IReadOnlyDictionary<string, string?> config)
    {
        return new UpdaterService(BuildConfig(config), feedClient, versionProvider, new NoOpAuditLogger());
    }

    private sealed class StubFeedClient(RemoteUpdateFeedSnapshot? snapshot) : IUpdateFeedClient
    {
        public Task<RemoteUpdateFeedSnapshot?> FetchAsync(string feedUrl, CancellationToken cancellationToken = default)
            => Task.FromResult(snapshot);
    }

    private sealed class FixedVersionProvider(string version) : ICurrentApplicationVersionProvider
    {
        public string GetCurrentVersion() => version;
    }

    private sealed class NoOpAuditLogger : IAuditLogger
    {
        public Task<Result> WriteAsync(
            string action,
            string details,
            CancellationToken cancellationToken = default,
            string? module = null,
            string? user = null)
            => Task.FromResult(Result.Success());
    }
}
