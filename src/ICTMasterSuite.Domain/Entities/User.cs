using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class User : EntityBase
{
    public string FullName { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    public User(string fullName, string username, string email, string passwordHash, Guid roleId)
    {
        FullName = fullName;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        IsActive = true;
    }

    public void UpdateProfile(string fullName, string username, string email, Guid roleId)
    {
        FullName = fullName;
        Username = username;
        Email = email;
        RoleId = roleId;
        Touch();
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
