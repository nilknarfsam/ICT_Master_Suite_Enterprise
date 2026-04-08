using FluentAssertions;
using ICTMasterSuite.Application.Common;

namespace ICTMasterSuite.Application.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        var result = Result.Success("ok");

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("ok");
    }
}
