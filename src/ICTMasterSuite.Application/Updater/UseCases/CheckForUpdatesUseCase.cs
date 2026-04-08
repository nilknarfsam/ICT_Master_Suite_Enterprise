using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Updater.Dtos;

namespace ICTMasterSuite.Application.Updater.UseCases;

public sealed class CheckForUpdatesUseCase(IUpdaterService updaterService)
{
    public Task<Result<VersionInfoDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => updaterService.CheckForUpdatesAsync(cancellationToken);
}
