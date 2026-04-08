namespace ICTMasterSuite.Application.Updater.Dtos;

public sealed record VersionInfoDto(string CurrentVersion, string LatestVersion, bool UpdateAvailable, string Notes, string DownloadUrl);
