using ClosedXML.Excel;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.Configuration.Dtos;
using ICTMasterSuite.Application.Dashboard.Dtos;
using ICTMasterSuite.Application.Reporting.Dtos;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class ReportingService(
    ITechnicalAnalysisRepository technicalRepository,
    IKnowledgeBaseArticleRepository knowledgeRepository,
    IAuditLogger auditLogger) : IReportingService
{
    private sealed record PdfMeta(string Title, string Subtitle, string SectionTitle);

    public async Task<Result<ReportFileDto>> ExportTechnicalHistoryAsync(TechnicalHistoryReportRequest request, CancellationToken cancellationToken = default)
    {
        var rows = await technicalRepository.ListBySerialAsync(request.SerialNumber, cancellationToken);
        var serial = request.SerialNumber.Trim();
        return await ExportAsync(
            request.Format,
            $"technical-history-{serial}",
            ["Data (UTC)", "Serial", "Modelo", "Resultado", "Erro", "Resumo"],
            rows.Select(x => new[] { x.CreatedAt.ToString("yyyy-MM-dd HH:mm"), x.SerialNumber, x.Model, x.Result.ToString(), x.ErrorCode, x.Summary }).ToList(),
            "technical-history.export",
            new PdfMeta(
                "Histórico técnico",
                $"Serial {serial}",
                "Registros de análises"),
            cancellationToken);
    }

    public async Task<Result<ReportFileDto>> ExportKnowledgeBaseAsync(KnowledgeBaseReportRequest request, CancellationToken cancellationToken = default)
    {
        var rows = await knowledgeRepository.SearchAsync(request.ModelFilter, request.TestPhaseFilter, request.TermFilter, true, cancellationToken);
        var subtitle = BuildKnowledgeBaseSubtitle(request);
        return await ExportAsync(
            request.Format,
            "knowledge-base",
            ["Modelo", "Fase", "Sintoma", "Solução", "Autor", "Ativo"],
            rows.Select(x => new[] { x.Model, x.TestPhase, x.Symptom, x.Solution, x.Author, x.IsActive ? "Sim" : "Não" }).ToList(),
            "knowledge-base.export",
            new PdfMeta(
                "Base de conhecimento",
                subtitle,
                "Artigos exportados"),
            cancellationToken);
    }

    public Task<Result<ReportFileDto>> ExportFinderResultsAsync(FinderResultsReportRequest request, CancellationToken cancellationToken = default)
    {
        return ExportAsync(
            request.Format,
            "finder-results",
            ["Arquivo", "Tipo", "Resultado", "Serial", "Modelo", "Erro", "Resumo"],
            request.Results.Select(x => new[] { x.FileName, x.SourceType, x.Result.ToString(), x.SerialNumber, x.Model, x.ErrorCode, x.Summary }).ToList(),
            "finder.export",
            new PdfMeta(
                "Finder de logs / Parser",
                "Resultados da última pesquisa em memória",
                "Arquivos analisados"),
            cancellationToken);
    }

    private static string BuildKnowledgeBaseSubtitle(KnowledgeBaseReportRequest request)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.ModelFilter))
        {
            parts.Add($"Modelo: {request.ModelFilter.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(request.TestPhaseFilter))
        {
            parts.Add($"Fase: {request.TestPhaseFilter.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(request.TermFilter))
        {
            parts.Add($"Termo: {request.TermFilter.Trim()}");
        }

        return parts.Count > 0 ? string.Join(" · ", parts) : "Todos os filtros (exportação ampla)";
    }

    private async Task<Result<ReportFileDto>> ExportAsync(
        ReportFormat format,
        string baseName,
        IReadOnlyCollection<string> headers,
        IReadOnlyCollection<string[]> rows,
        string auditAction,
        PdfMeta pdfMeta,
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
            var sheet = workbook.Worksheets.Add("Relatorio");
            for (var i = 0; i < headers.Count; i++)
            {
                sheet.Cell(1, i + 1).Value = headers.ElementAt(i);
            }

            var rowIndex = 2;
            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    sheet.Cell(rowIndex, i + 1).Value = row[i];
                }

                rowIndex++;
            }

            workbook.SaveAs(filePath);
        }
        else
        {
            var definition = new InstitutionalPdfDocumentBuilder.Definition(
                pdfMeta.Title,
                pdfMeta.Subtitle,
                pdfMeta.SectionTitle,
                headers.ToList(),
                rows.ToList(),
                DateTimeOffset.UtcNow);
            InstitutionalPdfDocumentBuilder.GeneratePdf(filePath, definition);
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
