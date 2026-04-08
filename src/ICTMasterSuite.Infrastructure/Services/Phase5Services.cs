using ClosedXML.Excel;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Configuration.Dtos;
using ICTMasterSuite.Application.Dashboard.Dtos;
using ICTMasterSuite.Application.Reporting.Dtos;
using ICTMasterSuite.Application.Updater.Dtos;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class ReportingService(
    ITechnicalAnalysisRepository technicalRepository,
    IKnowledgeBaseArticleRepository knowledgeRepository,
    IAuditLogger auditLogger) : IReportingService
{
    public async Task<Result<ReportFileDto>> ExportTechnicalHistoryAsync(TechnicalHistoryReportRequest request, CancellationToken cancellationToken = default)
    {
        var rows = await technicalRepository.ListBySerialAsync(request.SerialNumber, cancellationToken);
        return await ExportAsync(
            request.Format,
            $"technical-history-{request.SerialNumber}",
            ["Date", "Serial", "Model", "Result", "Error", "Summary"],
            rows.Select(x => new[] { x.CreatedAt.ToString("yyyy-MM-dd HH:mm"), x.SerialNumber, x.Model, x.Result.ToString(), x.ErrorCode, x.Summary }).ToList(),
            "technical-history.export",
            cancellationToken);
    }

    public async Task<Result<ReportFileDto>> ExportKnowledgeBaseAsync(KnowledgeBaseReportRequest request, CancellationToken cancellationToken = default)
    {
        var rows = await knowledgeRepository.SearchAsync(request.ModelFilter, request.TestPhaseFilter, request.TermFilter, true, cancellationToken);
        return await ExportAsync(
            request.Format,
            "knowledge-base",
            ["Model", "Phase", "Symptom", "Solution", "Author", "Active"],
            rows.Select(x => new[] { x.Model, x.TestPhase, x.Symptom, x.Solution, x.Author, x.IsActive ? "Yes" : "No" }).ToList(),
            "knowledge-base.export",
            cancellationToken);
    }

    public Task<Result<ReportFileDto>> ExportFinderResultsAsync(FinderResultsReportRequest request, CancellationToken cancellationToken = default)
    {
        return ExportAsync(
            request.Format,
            "finder-results",
            ["File", "Type", "Result", "Serial", "Model", "Error", "Summary"],
            request.Results.Select(x => new[] { x.FileName, x.SourceType, x.Result.ToString(), x.SerialNumber, x.Model, x.ErrorCode, x.Summary }).ToList(),
            "finder.export",
            cancellationToken);
    }

    private async Task<Result<ReportFileDto>> ExportAsync(
        ReportFormat format,
        string baseName,
        IReadOnlyCollection<string> headers,
        IReadOnlyCollection<string[]> rows,
        string auditAction,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory("reports");
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var filePath = format == ReportFormat.Excel
            ? Path.Combine("reports", $"{baseName}-{stamp}.xlsx")
            : Path.Combine("reports", $"{baseName}-{stamp}.pdf");

        if (format == ReportFormat.Excel)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            for (var i = 0; i < headers.Count; i++) sheet.Cell(1, i + 1).Value = headers.ElementAt(i);
            var rowIndex = 2;
            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++) sheet.Cell(rowIndex, i + 1).Value = row[i];
                rowIndex++;
            }
            workbook.SaveAs(filePath);
        }
        else
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(24);
                    page.Header().Text("ICT Master Suite Report").FontSize(18).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text(string.Join(" | ", headers)).FontColor(Colors.Grey.Darken2);
                        foreach (var row in rows)
                        {
                            col.Item().Text(string.Join(" | ", row));
                        }
                    });
                });
            }).GeneratePdf(filePath);
        }

        await auditLogger.WriteAsync(auditAction, $"Format: {format}; File: {filePath}", cancellationToken);
        return Result<ReportFileDto>.Success(new ReportFileDto(Path.GetFullPath(filePath), Path.GetFileName(filePath)));
    }
}

public sealed class DashboardService(
    ITechnicalAnalysisRepository technicalRepository,
    IKnowledgeBaseArticleRepository knowledgeRepository) : IDashboardService
{
    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var analyses = await technicalRepository.ListAllAsync(cancellationToken);
        var activeArticles = await knowledgeRepository.CountActiveAsync(cancellationToken);

        var failCount = analyses.Count(x => x.Result == LogResult.Fail);
        var passCount = analyses.Count(x => x.Result == LogResult.Pass);
        var topSerials = analyses
            .GroupBy(x => x.SerialNumber)
            .OrderByDescending(x => x.Count())
            .Take(5)
            .Select(x => new SerialRecurrenceDto(x.Key, x.Count()))
            .ToList();

        return Result<DashboardSummaryDto>.Success(new DashboardSummaryDto(
            analyses.Count,
            activeArticles,
            failCount,
            passCount,
            topSerials));
    }
}

public sealed class UpdaterService(IConfiguration configuration, IAuditLogger auditLogger) : IUpdaterService
{
    public async Task<Result<VersionInfoDto>> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        var current = "1.0.0";
        var latest = configuration["Updater:LatestVersion"] ?? current;
        var notes = configuration["Updater:Notes"] ?? "Nenhuma nota informada.";
        var url = configuration["Updater:DownloadUrl"] ?? string.Empty;
        var available = !string.Equals(current, latest, StringComparison.OrdinalIgnoreCase);

        await auditLogger.WriteAsync("updater.check", $"Current: {current}; Latest: {latest}; Available: {available}", cancellationToken);
        Log.Information("Updater check executado. Current={Current} Latest={Latest} Available={Available}", current, latest, available);
        return Result<VersionInfoDto>.Success(new VersionInfoDto(current, latest, available, notes, url));
    }
}

public sealed class SystemConfigurationService(
    ISystemSettingRepository repository,
    IAuditLogger auditLogger,
    Application.Abstractions.Infrastructure.IAppDbContext dbContext) : ISystemConfigurationService
{
    public async Task<Result<SystemConfigurationDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var rows = await repository.ListAllAsync(cancellationToken);
        string Get(string category, string key, string fallback)
            => rows.FirstOrDefault(x => x.Category == category && x.Key == key)?.Value ?? fallback;

        return Result<SystemConfigurationDto>.Success(new SystemConfigurationDto(
            Get("Finder", "Directories", ""),
            Get("Updater", "Endpoint", ""),
            bool.TryParse(Get("Updater", "AutoCheck", "true"), out var autoCheck) && autoCheck,
            Get("UI", "Theme", "Dark"),
            bool.TryParse(Get("UI", "AutoOpenLastModule", "true"), out var autoOpen) && autoOpen));
    }

    public async Task<Result> SaveAsync(SystemConfigurationDto configuration, CancellationToken cancellationToken = default)
    {
        await UpsertAsync("Finder", "Directories", configuration.FinderDirectories, cancellationToken);
        await UpsertAsync("Updater", "Endpoint", configuration.UpdaterEndpoint, cancellationToken);
        await UpsertAsync("Updater", "AutoCheck", configuration.AutoCheckUpdates.ToString(), cancellationToken);
        await UpsertAsync("UI", "Theme", configuration.PreferredTheme, cancellationToken);
        await UpsertAsync("UI", "AutoOpenLastModule", configuration.AutoOpenLastModule.ToString(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("settings.save", "Configuracoes do sistema atualizadas.", cancellationToken);
        return Result.Success("Configurações salvas.");
    }

    private async Task UpsertAsync(string category, string key, string value, CancellationToken cancellationToken)
    {
        var current = await repository.GetAsync(category, key, cancellationToken);
        if (current is null)
        {
            await repository.AddAsync(new SystemSetting(category, key, value), cancellationToken);
            return;
        }

        current.UpdateValue(value);
    }
}
