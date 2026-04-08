using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Dashboard.Dtos;
using ICTMasterSuite.Application.Dashboard.UseCases;
using ICTMasterSuite.Application.Reporting.Dtos;
using ICTMasterSuite.Application.Reporting.UseCases;
using ICTMasterSuite.Application.Updater.Dtos;
using ICTMasterSuite.Application.Updater.UseCases;

namespace ICTMasterSuite.Application.Tests;

public class Phase5UseCasesTests
{
    [Fact]
    public async Task GetDashboardSummaryUseCase_ShouldReturnSummary()
    {
        var useCase = new GetDashboardSummaryUseCase(new FakeDashboardService());
        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task ExportTechnicalHistoryReportUseCase_ShouldReturnFile()
    {
        var useCase = new ExportTechnicalHistoryReportUseCase(new FakeReportingService());
        var result = await useCase.ExecuteAsync(new TechnicalHistoryReportRequest("SN-1", ReportFormat.Excel));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task CheckForUpdatesUseCase_ShouldReturnVersionInfo()
    {
        var useCase = new CheckForUpdatesUseCase(new FakeUpdaterService());
        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value!.UpdateAvailable);
    }

    private sealed class FakeDashboardService : IDashboardService
    {
        public Task<Result<DashboardSummaryDto>> GetSummaryAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<DashboardSummaryDto>.Success(new DashboardSummaryDto(10, 4, 7, 3, [new SerialRecurrenceDto("SN-1", 3)])));
    }

    private sealed class FakeReportingService : IReportingService
    {
        public Task<Result<ReportFileDto>> ExportTechnicalHistoryAsync(TechnicalHistoryReportRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<ReportFileDto>.Success(new ReportFileDto("c:\\temp\\a.xlsx", "a.xlsx")));

        public Task<Result<ReportFileDto>> ExportKnowledgeBaseAsync(KnowledgeBaseReportRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<ReportFileDto>.Failure("not used"));

        public Task<Result<ReportFileDto>> ExportFinderResultsAsync(FinderResultsReportRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<ReportFileDto>.Failure("not used"));
    }

    private sealed class FakeUpdaterService : IUpdaterService
    {
        public Task<Result<VersionInfoDto>> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<VersionInfoDto>.Success(new VersionInfoDto("1.0.0", "1.1.0", true, "notes", "https://example.com")));
    }
}
