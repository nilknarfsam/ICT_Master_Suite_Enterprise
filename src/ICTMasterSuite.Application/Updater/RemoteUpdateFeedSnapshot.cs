namespace ICTMasterSuite.Application.Updater;

/// <summary>Estado normalizado do feed remoto após parse (campos opcionais podem ser nulos).</summary>
public sealed record RemoteUpdateFeedSnapshot(
    string LatestVersion,
    string? ReleaseNotes,
    string? DownloadUrl,
    DateTimeOffset? PublishedAt,
    string? IntegritySha256,
    string? IntegritySignatureBase64);
