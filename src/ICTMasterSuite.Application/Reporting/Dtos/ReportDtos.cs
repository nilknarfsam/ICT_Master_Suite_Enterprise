using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Reporting.Dtos;

public enum ReportFormat
{
    Excel = 1,
    Pdf = 2
}

public sealed record ReportFileDto(string FilePath, string DisplayName);

public sealed record TechnicalHistoryReportRequest(string SerialNumber, ReportFormat Format);

public sealed record KnowledgeBaseReportRequest(string? ModelFilter, string? TestPhaseFilter, string? TermFilter, ReportFormat Format);

public sealed record FinderResultsReportRequest(IReadOnlyCollection<ParsedLog> Results, ReportFormat Format);
