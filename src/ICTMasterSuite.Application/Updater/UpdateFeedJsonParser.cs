using System.Text.Json;
using System.Text.Json.Serialization;

namespace ICTMasterSuite.Application.Updater;

/// <summary>Parse puro do JSON do feed (testável sem HTTP).</summary>
public static class UpdateFeedJsonParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>Interpreta o JSON do feed. Retorna <c>null</c> se o JSON for inválido ou <c>latestVersion</c> estiver ausente/vazio.</summary>
    public static RemoteUpdateFeedSnapshot? TryParse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var feed = JsonSerializer.Deserialize<FeedDto>(json, JsonOptions);
            if (feed is null || string.IsNullOrWhiteSpace(feed.LatestVersion))
            {
                return null;
            }

            var latest = feed.LatestVersion.Trim();
            return new RemoteUpdateFeedSnapshot(
                latest,
                string.IsNullOrWhiteSpace(feed.ReleaseNotes) ? null : feed.ReleaseNotes.Trim(),
                string.IsNullOrWhiteSpace(feed.DownloadUrl) ? null : feed.DownloadUrl.Trim(),
                feed.PublishedAt,
                string.IsNullOrWhiteSpace(feed.Integrity?.Sha256) ? null : feed.Integrity!.Sha256.Trim(),
                string.IsNullOrWhiteSpace(feed.Integrity?.SignatureBase64) ? null : feed.Integrity!.SignatureBase64.Trim());
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed class FeedDto
    {
        [JsonPropertyName("latestVersion")]
        public string? LatestVersion { get; set; }

        [JsonPropertyName("releaseNotes")]
        public string? ReleaseNotes { get; set; }

        [JsonPropertyName("downloadUrl")]
        public string? DownloadUrl { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTimeOffset? PublishedAt { get; set; }

        [JsonPropertyName("integrity")]
        public IntegrityDto? Integrity { get; set; }
    }

    private sealed class IntegrityDto
    {
        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("signatureBase64")]
        public string? SignatureBase64 { get; set; }
    }
}
