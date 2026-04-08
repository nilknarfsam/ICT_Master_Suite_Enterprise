using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Deactivate_ShouldMarkUserAsInactive()
    {
        var roleId = Guid.NewGuid();
        var user = new User("Test User", "test.user", "test@ict.local", "hash", roleId);

        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_ShouldMarkUserAsActive()
    {
        var roleId = Guid.NewGuid();
        var user = new User("Test User", "test.user", "test@ict.local", "hash", roleId);
        user.Deactivate();

        user.Activate();

        Assert.True(user.IsActive);
    }

    [Fact]
    public void ChangePassword_ShouldClearMustChangePassword()
    {
        var roleId = Guid.NewGuid();
        var user = new User("Test User", "test.user", "test@ict.local", "hash", roleId, mustChangePassword: true);

        Assert.True(user.MustChangePassword);
        user.ChangePassword("new-hash");

        Assert.False(user.MustChangePassword);
    }
}
