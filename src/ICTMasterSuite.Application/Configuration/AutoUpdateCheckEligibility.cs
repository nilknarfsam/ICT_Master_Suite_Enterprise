namespace ICTMasterSuite.Application.Configuration;

/// <summary>
/// Encapsulates when the shell may run an automatic update check after settings are loaded.
/// Automatic checks must not run when the operator disabled "AutoCheck" in system configuration.
/// </summary>
public static class AutoUpdateCheckEligibility
{
    public static bool ShouldRunAutomaticCheck(bool autoCheckUpdatesFromLoadedConfiguration)
        => autoCheckUpdatesFromLoadedConfiguration;
}
