using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class Role : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ICollection<RolePermission> RolePermissions { get; private set; } = [];

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        Touch();
    }
}
