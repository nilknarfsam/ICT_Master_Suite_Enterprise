using ICTMasterSuite.Domain.Common;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Domain.Entities;

public sealed class Permission : EntityBase
{
    public SystemModule Module { get; private set; }
    public PermissionAction Action { get; private set; }
    public string Name { get; private set; }
    public ICollection<RolePermission> RolePermissions { get; private set; } = [];

    public Permission(SystemModule module, PermissionAction action, string name)
    {
        Module = module;
        Action = action;
        Name = name;
    }
}
