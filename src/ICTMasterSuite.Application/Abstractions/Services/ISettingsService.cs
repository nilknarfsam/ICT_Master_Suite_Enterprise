using ICTMasterSuite.Application.Configuration;

namespace ICTMasterSuite.Application.Abstractions.Services;

public interface ISettingsService
{
    Task<SystemSettings> LoadAsync();
    Task SaveAsync(SystemSettings settings);
}
