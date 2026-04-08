namespace ICTMasterSuite.Application.Dashboard.Dtos;

public sealed record DashboardSummaryDto(
    int TotalAnalyses,
    int ActiveKnowledgeArticles,
    int FailCount,
    int PassCount,
    IReadOnlyCollection<SerialRecurrenceDto> TopRecurringSerials);

public sealed record SerialRecurrenceDto(string SerialNumber, int Occurrences);
