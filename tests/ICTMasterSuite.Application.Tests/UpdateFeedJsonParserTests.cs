using FluentAssertions;
using ICTMasterSuite.Application.Updater;

namespace ICTMasterSuite.Application.Tests;

public class UpdateFeedJsonParserTests
{
    [Fact]
    public void TryParse_ValidFullJson_ReturnsSnapshotWithAllFields()
    {
        const string json = """
            {
              "latestVersion": "2.1.0",
              "releaseNotes": "Notas",
              "downloadUrl": "https://x/setup.exe",
              "publishedAt": "2026-04-08T12:00:00+00:00",
              "integrity": { "sha256": "abc", "signatureBase64": "sig" }
            }
            """;

        var r = UpdateFeedJsonParser.TryParse(json);

        r.Should().NotBeNull();
        r!.LatestVersion.Should().Be("2.1.0");
        r.ReleaseNotes.Should().Be("Notas");
        r.DownloadUrl.Should().Be("https://x/setup.exe");
        r.PublishedAt.Should().NotBeNull();
        r.IntegritySha256.Should().Be("abc");
        r.IntegritySignatureBase64.Should().Be("sig");
    }

    [Fact]
    public void TryParse_OnlyLatestVersion_OptionalFieldsNull()
    {
        const string json = """{ "latestVersion": "1.0.1" }""";

        var r = UpdateFeedJsonParser.TryParse(json);

        r.Should().NotBeNull();
        r!.LatestVersion.Should().Be("1.0.1");
        r.ReleaseNotes.Should().BeNull();
        r.DownloadUrl.Should().BeNull();
        r.PublishedAt.Should().BeNull();
        r.IntegritySha256.Should().BeNull();
        r.IntegritySignatureBase64.Should().BeNull();
    }

    [Fact]
    public void TryParse_InvalidJson_ReturnsNull()
    {
        UpdateFeedJsonParser.TryParse("{ not json").Should().BeNull();
    }

    [Fact]
    public void TryParse_MissingLatestVersion_ReturnsNull()
    {
        const string json = """{ "releaseNotes": "x" }""";

        UpdateFeedJsonParser.TryParse(json).Should().BeNull();
    }

    [Fact]
    public void TryParse_EmptyLatestVersion_ReturnsNull()
    {
        const string json = """{ "latestVersion": "   " }""";

        UpdateFeedJsonParser.TryParse(json).Should().BeNull();
    }

    [Fact]
    public void TryParse_CaseInsensitiveProperties_Works()
    {
        const string json = """{ "LatestVersion": "3.0.0", "DownloadURL": "https://u" }""";

        var r = UpdateFeedJsonParser.TryParse(json);

        r.Should().NotBeNull();
        r!.LatestVersion.Should().Be("3.0.0");
        r.DownloadUrl.Should().Be("https://u");
    }
}
