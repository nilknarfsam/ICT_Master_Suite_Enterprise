using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Configuration.Dtos;
using ICTMasterSuite.Application.Dashboard.Dtos;
using ICTMasterSuite.Application.Reporting.Dtos;
using ICTMasterSuite.Application.Updater.Dtos;

namespace ICTMasterSuite.Application.Abstractions.Services;

public interface IReportingService
{
    Task<Result<ReportFileDto>> ExportTechnicalHistoryAsync(TechnicalHistoryReportRequest request, CancellationToken cancellationToken = default);
    Task<Result<ReportFileDto>> ExportKnowledgeBaseAsync(KnowledgeBaseReportRequest request, CancellationToken cancellationToken = default);
    Task<Result<ReportFileDto>> ExportFinderResultsAsync(FinderResultsReportRequest request, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync(CancellationToken cancellationToken = default);
}

public interface IUpdaterService
{
    Task<Result<VersionInfoDto>> CheckForUpdatesAsync(UpdaterCheckRequest? request = null, CancellationToken cancellationToken = default);
}

public interface ISystemConfigurationService
{
    Task<Result<SystemConfigurationDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result> SaveAsync(SystemConfigurationDto configuration, CancellationToken cancellationToken = default);
}
