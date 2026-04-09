using System.Text.Json;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Configuration;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class JsonSettingsService : ISettingsService
{
    private const string SettingsFileName = "settings.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _settingsFilePath;

    public JsonSettingsService()
    {
        _settingsFilePath = BuildSettingsFilePath();
    }

    public async Task<SystemSettings> LoadAsync()
    {
        if (!File.Exists(_settingsFilePath))
        {
            var defaults = new SystemSettings();
            await SaveAsync(defaults);
            return defaults;
        }

        await using var stream = File.OpenRead(_settingsFilePath);
        var settings = await JsonSerializer.DeserializeAsync<SystemSettings>(stream, JsonOptions);
        return settings ?? new SystemSettings();
    }

    public async Task SaveAsync(SystemSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_settingsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_settingsFilePath);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions);
    }

    private static string BuildSettingsFilePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!string.IsNullOrWhiteSpace(appDataPath))
        {
            return Path.Combine(appDataPath, "ICTMasterSuite", SettingsFileName);
        }

        return Path.Combine(AppContext.BaseDirectory, SettingsFileName);
    }
}
