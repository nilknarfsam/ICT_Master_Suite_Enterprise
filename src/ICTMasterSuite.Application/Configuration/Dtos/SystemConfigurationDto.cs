namespace ICTMasterSuite.Application.Configuration.Dtos;

public sealed record SystemConfigurationDto(
    string FinderDirectories,
    string UpdaterEndpoint,
    bool AutoCheckUpdates,
    string PreferredTheme,
    bool AutoOpenLastModule);
