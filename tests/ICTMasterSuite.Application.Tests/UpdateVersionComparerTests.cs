using FluentAssertions;
using ICTMasterSuite.Application.Updater;

namespace ICTMasterSuite.Application.Tests;

public class UpdateVersionComparerTests
{
    [Theory]
    [InlineData("1.0.0", "1.1.0", true)]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("1.0.0", "0.9.9", false)]
    [InlineData("v1.2.0", "1.2.1", true)]
    public void IsRemoteNewer_SemanticVersions_MatchesExpected(string current, string latest, bool expected)
    {
        UpdateVersionComparer.IsRemoteNewer(current, latest).Should().Be(expected);
    }

    [Fact]
    public void IsRemoteNewer_NonSemanticDifferentStrings_TreatedAsNewer()
    {
        UpdateVersionComparer.IsRemoteNewer("1.0.0", "custom-build").Should().BeTrue();
    }

    [Fact]
    public void NormalizeVersion_TrimsVPrefix()
    {
        UpdateVersionComparer.NormalizeVersion("v2.3.4").Should().Be("2.3.4");
    }
}
