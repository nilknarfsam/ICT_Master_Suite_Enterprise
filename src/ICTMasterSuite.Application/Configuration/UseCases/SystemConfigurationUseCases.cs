using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Configuration.Dtos;

namespace ICTMasterSuite.Application.Configuration.UseCases;

public sealed class GetSystemConfigurationUseCase(ISystemConfigurationService service)
{
    public Task<Result<SystemConfigurationDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => service.GetAsync(cancellationToken);
}

public sealed class SaveSystemConfigurationUseCase(ISystemConfigurationService service)
{
    public Task<Result> ExecuteAsync(SystemConfigurationDto configuration, CancellationToken cancellationToken = default)
        => service.SaveAsync(configuration, cancellationToken);
}
