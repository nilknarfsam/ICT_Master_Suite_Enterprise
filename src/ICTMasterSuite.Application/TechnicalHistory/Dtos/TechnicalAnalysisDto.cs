using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Application.TechnicalHistory.Dtos;

public sealed record TechnicalAnalysisDto(
    Guid Id,
    string SerialNumber,
    string Model,
    string FileName,
    string FilePath,
    string LogType,
    LogResult Result,
    string ErrorCode,
    string ErrorDescription,
    string Summary,
    string TechnicianName,
    string AnalysisText,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
