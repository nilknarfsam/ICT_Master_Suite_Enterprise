using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Infrastructure.Services;

namespace ICTMasterSuite.Application.Tests;

public class LogParserServiceTests
{
    [Fact]
    public async Task ParseAsync_ShouldExtractBasicFields()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"tri-log-{Guid.NewGuid():N}.log");
        await File.WriteAllTextAsync(filePath, "TRI\nserial:SN123\nmodel:MX100\nerror:E45\nstatus:FAIL");

        try
        {
            var parser = new LogParserService();
            var parsed = await parser.ParseAsync(new LogFile(Path.GetFileName(filePath), filePath, ".log", DateTime.UtcNow));

            Assert.Equal("TRI", parsed.SourceType);
            Assert.Equal("SN123", parsed.SerialNumber);
            Assert.Equal("MX100", parsed.Model);
            Assert.Equal("E45", parsed.ErrorCode);
            Assert.Equal(LogResult.Fail, parsed.Result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
