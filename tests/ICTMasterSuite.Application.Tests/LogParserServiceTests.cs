using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Infrastructure.Services;

namespace ICTMasterSuite.Application.Tests;

public class LogParserServiceTests
{
    [Fact]
    public async Task ParseAsync_ShouldParseTriLogWithHigherPrecision()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"tri-log-{Guid.NewGuid():N}.log");
        await File.WriteAllTextAsync(filePath,
            "TRI Functional Test\nTimestamp: 2026-04-08 10:20:30\nSerial Number: SN12345\nModel: MX100\nStation ID: TRI-ST01\nError Code: E45\nError Description: Voltage out of range\nResult: FAIL");

        try
        {
            var parser = new LogParserService();
            var parsed = await parser.ParseAsync(new LogFile(Path.GetFileName(filePath), filePath, ".log", DateTime.UtcNow));

            Assert.Equal("TRI", parsed.SourceType);
            Assert.Equal("SN12345", parsed.SerialNumber);
            Assert.Equal("MX100", parsed.Model);
            Assert.Equal("TRI-ST01", parsed.Station);
            Assert.Equal("E45", parsed.ErrorCode);
            Assert.Equal("Voltage out of range", parsed.ErrorDescription);
            Assert.Equal(LogResult.Fail, parsed.Result);
            Assert.Equal(new DateTime(2026, 4, 8, 10, 20, 30), parsed.LogTimestamp);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ParseAsync_ShouldParseAgilentLogWithHigherPrecision()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"agilent-log-{Guid.NewGuid():N}.csv");
        await File.WriteAllTextAsync(filePath,
            "Agilent ICT\nStart Time=2026-04-08 09:05:00\nUUT Serial=AG987\nUUT Model=N6700\nTester=AG-ST02\nFailure Code=F901\nFailure Message=Open circuit at pin 3\nStatus=FAILED");

        try
        {
            var parser = new LogParserService();
            var parsed = await parser.ParseAsync(new LogFile(Path.GetFileName(filePath), filePath, ".csv", DateTime.UtcNow));

            Assert.Equal("Agilent", parsed.SourceType);
            Assert.Equal("AG987", parsed.SerialNumber);
            Assert.Equal("N6700", parsed.Model);
            Assert.Equal("AG-ST02", parsed.Station);
            Assert.Equal("F901", parsed.ErrorCode);
            Assert.Equal("Open circuit at pin 3", parsed.ErrorDescription);
            Assert.Equal(LogResult.Fail, parsed.Result);
            Assert.Equal(new DateTime(2026, 4, 8, 9, 5, 0), parsed.LogTimestamp);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ParseAsync_ShouldFallbackForEmptyOrInvalidContent()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"invalid-log-{Guid.NewGuid():N}.txt");
        await File.WriteAllTextAsync(filePath, string.Empty);

        try
        {
            var parser = new LogParserService();
            var parsed = await parser.ParseAsync(new LogFile(Path.GetFileName(filePath), filePath, ".txt", DateTime.UtcNow));

            Assert.Equal("Unknown", parsed.SourceType);
            Assert.Equal("N/A", parsed.SerialNumber);
            Assert.Equal("N/A", parsed.ErrorCode);
            Assert.NotNull(parsed.Summary);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
