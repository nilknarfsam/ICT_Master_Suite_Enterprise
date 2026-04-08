using ICTMasterSuite.Infrastructure.Services;

namespace ICTMasterSuite.Application.Tests;

public class LogFinderServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldFilterByExtensionNameAndPassRules()
    {
        var root = Path.Combine(Path.GetTempPath(), $"ict-finder-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var valid = Path.Combine(root, "board_fail_001.log");
            var ignoredPass = Path.Combine(root, "board_pass_001.log");
            var ignoredPrefix = Path.Combine(root, "p_board_001.txt");
            var ignoredExt = Path.Combine(root, "board_fail_001.json");

            await File.WriteAllTextAsync(valid, "dummy");
            await File.WriteAllTextAsync(ignoredPass, "dummy");
            await File.WriteAllTextAsync(ignoredPrefix, "dummy");
            await File.WriteAllTextAsync(ignoredExt, "dummy");

            var service = new LogFinderService();
            var result = await service.SearchAsync("fail", [root]);

            Assert.Single(result);
            Assert.Equal("board_fail_001.log", result.First().FileName);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public async Task SearchAsync_ShouldAvoidDuplicatesWhenDirectoriesOverlap()
    {
        var root = Path.Combine(Path.GetTempPath(), $"ict-finder-overlap-{Guid.NewGuid():N}");
        var sub = Path.Combine(root, "sub");
        Directory.CreateDirectory(sub);

        try
        {
            var valid = Path.Combine(sub, "tri_fail_01.log");
            await File.WriteAllTextAsync(valid, "dummy");

            var service = new LogFinderService();
            var result = await service.SearchAsync("fail", [root, sub]);

            Assert.Single(result);
            Assert.Equal(Path.GetFullPath(valid), result.First().FullPath);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
