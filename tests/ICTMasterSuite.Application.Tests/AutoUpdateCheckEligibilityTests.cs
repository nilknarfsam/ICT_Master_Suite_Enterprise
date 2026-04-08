using ICTMasterSuite.Application.Configuration;

namespace ICTMasterSuite.Application.Tests;

public class AutoUpdateCheckEligibilityTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ShouldRunAutomaticCheck_MatchesLoadedConfiguration(bool autoCheckInSettings, bool expected)
    {
        var actual = AutoUpdateCheckEligibility.ShouldRunAutomaticCheck(autoCheckInSettings);
        Assert.Equal(expected, actual);
    }
}
