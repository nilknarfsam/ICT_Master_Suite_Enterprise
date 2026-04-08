using System.Text.Json.Serialization;

namespace ICTMasterSuite.Infrastructure.Services;

/// <summary>Contrato JSON do feed remoto de versão (extensível).</summary>
internal sealed class RemoteUpdateFeedDto
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
    public RemoteIntegrityDto? Integrity { get; set; }
}

internal sealed class RemoteIntegrityDto
{
    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }

    [JsonPropertyName("signatureBase64")]
    public string? SignatureBase64 { get; set; }
}
