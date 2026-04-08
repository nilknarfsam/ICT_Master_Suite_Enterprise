using ICTMasterSuite.Domain.Common;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Domain.Entities;

public sealed class TechnicalAnalysis : EntityBase
{
    public string SerialNumber { get; private set; }
    public string Model { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string LogType { get; private set; }
    public LogResult Result { get; private set; }
    public string ErrorCode { get; private set; }
    public string ErrorDescription { get; private set; }
    public string Summary { get; private set; }
    public string TechnicianName { get; private set; }
    public string AnalysisText { get; private set; }

    public TechnicalAnalysis(
        string serialNumber,
        string model,
        string fileName,
        string filePath,
        string logType,
        LogResult result,
        string errorCode,
        string errorDescription,
        string summary,
        string technicianName,
        string analysisText)
    {
        SerialNumber = serialNumber;
        Model = model;
        FileName = fileName;
        FilePath = filePath;
        LogType = logType;
        Result = result;
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
        Summary = summary;
        TechnicianName = technicianName;
        AnalysisText = analysisText;
    }

    public void UpdateAnalysis(string analysisText, string technicianName)
    {
        AnalysisText = analysisText;
        TechnicianName = technicianName;
        Touch();
    }
}
