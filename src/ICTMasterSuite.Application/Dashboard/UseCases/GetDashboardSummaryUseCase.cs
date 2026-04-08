using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Dashboard.Dtos;

namespace ICTMasterSuite.Application.Dashboard.UseCases;

public sealed class GetDashboardSummaryUseCase(IDashboardService dashboardService)
{
    public Task<Result<DashboardSummaryDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => dashboardService.GetSummaryAsync(cancellationToken);
}
