using System.Text;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class LogParserService : ILogParserService
{
    public async Task<ParsedLog> ParseAsync(LogFile logFile, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = await ReadSafeAsync(logFile.FullPath, cancellationToken);
            var sourceType = DetectSourceType(logFile.FileName, content);
            var result = DetectResult(content, logFile.FileName);
            var serial = ExtractValue(content, ["serial", "sn", "serialnumber"]);
            var model = ExtractValue(content, ["model", "pn", "partnumber"]);
            var error = ExtractValue(content, ["error", "errorcode", "fault"]);
            var summary = BuildSummary(sourceType, result, serial, model, error);

            return new ParsedLog(
                sourceType,
                logFile.FileName,
                logFile.FullPath,
                serial,
                model,
                error,
                result,
                DateTime.UtcNow,
                summary);
        }
        catch (Exception ex)
        {
            return new ParsedLog(
                "Unknown",
                logFile.FileName,
                logFile.FullPath,
                "N/A",
                "N/A",
                "READ_ERROR",
                LogResult.Fail,
                DateTime.UtcNow,
                $"Falha ao processar arquivo: {ex.Message}");
        }
    }

    private static async Task<string> ReadSafeAsync(string path, CancellationToken cancellationToken)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);
        using var reader = new StreamReader(stream, Encoding.UTF8, true);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static string DetectSourceType(string fileName, string content)
    {
        if (fileName.Contains("tri", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("tri", StringComparison.OrdinalIgnoreCase))
        {
            return "TRI";
        }

        if (fileName.Contains("agilent", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("agilent", StringComparison.OrdinalIgnoreCase))
        {
            return "Agilent";
        }

        return "Unknown";
    }

    private static LogResult DetectResult(string content, string fileName)
    {
        if (content.Contains("fail", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("error", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            return LogResult.Fail;
        }

        return LogResult.Pass;
    }

    private static string ExtractValue(string content, IEnumerable<string> keys)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            foreach (var key in keys)
            {
                if (!line.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var parts = line.Split([':', '=', ';'], 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]))
                {
                    return parts[1];
                }
            }
        }

        return "N/A";
    }

    private static string BuildSummary(string sourceType, LogResult result, string serial, string model, string errorCode)
    {
        return $"{sourceType} | {result} | SN: {serial} | Model: {model} | Error: {errorCode}";
    }
}
