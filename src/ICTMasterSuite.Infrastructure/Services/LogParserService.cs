using System.Text;
using System.Text.RegularExpressions;
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
            var sourceType = DetectSourceType(logFile.FileName, content, logFile.Extension);

            return sourceType switch
            {
                "TRI" => await ParseTriLogAsync(logFile, content, cancellationToken),
                "Agilent" => await ParseAgilentLogAsync(logFile, content, cancellationToken),
                _ => BuildUnknownParsedLog(logFile, content)
            };
        }
        catch (Exception ex)
        {
            return new ParsedLog(
                "Unknown",
                logFile.FileName,
                logFile.FullPath,
                "N/A",
                "N/A",
                "N/A",
                null,
                "READ_ERROR",
                ex.GetType().Name,
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

    private static async Task<ParsedLog> ParseTriLogAsync(LogFile logFile, string content, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        var serial = ExtractValue(content, ["serial number", "serialnumber", "serial", "sn"]);
        var model = ExtractValue(content, ["model number", "model", "part number", "pn"]);
        var station = ExtractValue(content, ["station id", "station", "test station"]);
        var errorCode = ExtractValue(content, ["error code", "errorcode", "fault code"]);
        var errorDescription = ExtractValue(content, ["error description", "errordescription", "fault description", "message"]);
        var timestamp = ExtractTimestamp(content, ["timestamp", "date", "datetime", "time"]);
        var result = DetectResult(content, logFile.FileName);
        var summary = BuildSummary("TRI", result, serial, model, station, errorCode, errorDescription);

        return new ParsedLog(
            "TRI",
            logFile.FileName,
            logFile.FullPath,
            serial,
            model,
            station,
            timestamp,
            errorCode,
            errorDescription,
            result,
            DateTime.UtcNow,
            summary);
    }

    private static async Task<ParsedLog> ParseAgilentLogAsync(LogFile logFile, string content, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        var serial = ExtractValue(content, ["uut serial", "serial", "sn", "barcode"]);
        var model = ExtractValue(content, ["uut model", "model", "product", "pn"]);
        var station = ExtractValue(content, ["tester", "station", "fixture"]);
        var errorCode = ExtractValue(content, ["error code", "failure code", "code"]);
        var errorDescription = ExtractValue(content, ["failure message", "error text", "description", "message"]);
        var timestamp = ExtractTimestamp(content, ["start time", "timestamp", "date", "time"]);
        var result = DetectResult(content, logFile.FileName);
        var summary = BuildSummary("Agilent", result, serial, model, station, errorCode, errorDescription);

        return new ParsedLog(
            "Agilent",
            logFile.FileName,
            logFile.FullPath,
            serial,
            model,
            station,
            timestamp,
            errorCode,
            errorDescription,
            result,
            DateTime.UtcNow,
            summary);
    }

    private static ParsedLog BuildUnknownParsedLog(LogFile logFile, string content)
    {
        var result = DetectResult(content, logFile.FileName);
        return new ParsedLog(
            "Unknown",
            logFile.FileName,
            logFile.FullPath,
            ExtractValue(content, ["serial", "sn"]),
            ExtractValue(content, ["model", "pn"]),
            ExtractValue(content, ["station"]),
            ExtractTimestamp(content, ["timestamp", "date", "time"]),
            ExtractValue(content, ["error code", "error", "fault"]),
            ExtractValue(content, ["error description", "description", "message"]),
            result,
            DateTime.UtcNow,
            BuildSummary("Unknown", result, "N/A", "N/A", "N/A", "N/A", "N/A"));
    }

    private static string DetectSourceType(string fileName, string content, string extension)
    {
        var triScore = 0;
        var agilentScore = 0;

        if (fileName.Contains("tri", StringComparison.OrdinalIgnoreCase)) triScore += 3;
        if (fileName.Contains("agilent", StringComparison.OrdinalIgnoreCase)) agilentScore += 3;
        if (extension.Equals(".dcl", StringComparison.OrdinalIgnoreCase)) triScore += 1;
        if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase)) agilentScore += 1;

        triScore += CountMatches(content, ["TRI", "TestResult", "station id", "serial number"]);
        agilentScore += CountMatches(content, ["Agilent", "UUT", "tester", "failure message"]);

        if (triScore == 0 && agilentScore == 0)
        {
            return "Unknown";
        }

        if (triScore >= agilentScore)
        {
            return "TRI";
        }

        return "Agilent";
    }

    private static LogResult DetectResult(string content, string fileName)
    {
        if (ContainsAny(content, ["fail", "failed", "error", "ng", "nok"]) ||
            fileName.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            return LogResult.Fail;
        }

        if (ContainsAny(content, ["pass", "passed", "ok"]))
        {
            return LogResult.Pass;
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

                var parts = line.Split([':', '=', ';', ','], 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[1]))
                {
                    return parts[1];
                }

                var regex = Regex.Match(line, $"{Regex.Escape(key)}\\s+(.+)$", RegexOptions.IgnoreCase);
                if (regex.Success)
                {
                    return regex.Groups[1].Value.Trim();
                }
            }
        }

        return "N/A";
    }

    private static DateTime? ExtractTimestamp(string content, IEnumerable<string> keys)
    {
        var value = ExtractValue(content, keys);
        if (value == "N/A")
        {
            return null;
        }

        return DateTime.TryParse(value, out var timestamp) ? timestamp : null;
    }

    private static string BuildSummary(
        string sourceType,
        LogResult result,
        string serial,
        string model,
        string station,
        string errorCode,
        string errorDescription)
    {
        return $"{sourceType} | {result} | SN: {serial} | Model: {model} | Station: {station} | Error: {errorCode} {errorDescription}".Trim();
    }

    private static int CountMatches(string content, IEnumerable<string> terms)
    {
        var score = 0;
        foreach (var term in terms)
        {
            if (content.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                score++;
            }
        }

        return score;
    }

    private static bool ContainsAny(string content, IEnumerable<string> terms)
    {
        return terms.Any(term => content.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
