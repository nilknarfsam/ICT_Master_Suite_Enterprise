using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Reporting.Dtos;

namespace ICTMasterSuite.Application.Reporting.UseCases;

public sealed class ExportTechnicalHistoryReportUseCase(IReportingService reportingService)
{
    public Task<Result<ReportFileDto>> ExecuteAsync(TechnicalHistoryReportRequest request, CancellationToken cancellationToken = default)
        => reportingService.ExportTechnicalHistoryAsync(request, cancellationToken);
}

public sealed class ExportKnowledgeBaseReportUseCase(IReportingService reportingService)
{
    public Task<Result<ReportFileDto>> ExecuteAsync(KnowledgeBaseReportRequest request, CancellationToken cancellationToken = default)
        => reportingService.ExportKnowledgeBaseAsync(request, cancellationToken);
}

public sealed class ExportFinderResultsReportUseCase(IReportingService reportingService)
{
    public Task<Result<ReportFileDto>> ExecuteAsync(FinderResultsReportRequest request, CancellationToken cancellationToken = default)
        => reportingService.ExportFinderResultsAsync(request, cancellationToken);
}
