using ICTMasterSuite.Domain.Common;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Domain.Entities;

public sealed class ParsedLog : EntityBase
{
    public string SourceType { get; private set; }
    public string FileName { get; private set; }
    public string FullPath { get; private set; }
    public string SerialNumber { get; private set; }
    public string Model { get; private set; }
    public string ErrorCode { get; private set; }
    public LogResult Result { get; private set; }
    public DateTime AnalysedAt { get; private set; }
    public string Summary { get; private set; }

    public ParsedLog(
        string sourceType,
        string fileName,
        string fullPath,
        string serialNumber,
        string model,
        string errorCode,
        LogResult result,
        DateTime analysedAt,
        string summary)
    {
        SourceType = sourceType;
        FileName = fileName;
        FullPath = fullPath;
        SerialNumber = serialNumber;
        Model = model;
        ErrorCode = errorCode;
        Result = result;
        AnalysedAt = analysedAt;
        Summary = summary;
    }
}
