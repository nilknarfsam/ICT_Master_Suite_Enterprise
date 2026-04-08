using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence;

internal static class SeedData
{
    internal static readonly Guid AdminRoleId = Guid.Parse("DFB4B5B2-4461-44A6-BCC5-6A2EA5A86F55");
    internal static readonly Guid TechnicianRoleId = Guid.Parse("2BA5CCDF-6F97-4E5D-9ABE-C2DCD4C66355");
    internal static readonly Guid AdminUserId = Guid.Parse("B8DF2F8A-10AF-4A6B-9B95-C485E43A1A14");

    internal static void Apply(ModelBuilder modelBuilder)
    {
        var permissions = BuildPermissions();
        var rolePermissions = BuildRolePermissions(permissions.Select(x => (x.Id, x.Action)).ToArray());

        modelBuilder.Entity<Role>().HasData(
            new { Id = AdminRoleId, Name = "Administrador", Description = "Acesso total ao sistema", CreatedAt = DateTime.UtcNow },
            new { Id = TechnicianRoleId, Name = "Tecnico", Description = "Acesso operacional tecnico", CreatedAt = DateTime.UtcNow });

        modelBuilder.Entity<Permission>().HasData(permissions);
        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

        var adminPasswordHash = PasswordHasher.HashStatic("Admin@123");
        modelBuilder.Entity<User>().HasData(new
        {
            Id = AdminUserId,
            FullName = "Administrador do Sistema",
            Username = "admin",
            Email = "admin@ict.local",
            PasswordHash = adminPasswordHash,
            IsActive = true,
            RoleId = AdminRoleId,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static PermissionSeed[] BuildPermissions()
    {
        var list = new List<PermissionSeed>();
        foreach (var module in Enum.GetValues<SystemModule>())
        {
            list.Add(new PermissionSeed(BuildPermissionId(module, PermissionAction.View), module, PermissionAction.View, $"{module}.View", DateTime.UtcNow));
            list.Add(new PermissionSeed(BuildPermissionId(module, PermissionAction.Manage), module, PermissionAction.Manage, $"{module}.Manage", DateTime.UtcNow));
        }

        return list.ToArray();
    }

    private static object[] BuildRolePermissions((Guid Id, PermissionAction Action)[] permissions)
    {
        var rows = new List<object>();
        foreach (var permission in permissions)
        {
            rows.Add(new { RoleId = AdminRoleId, PermissionId = permission.Id });
            if (permission.Action == PermissionAction.View)
            {
                rows.Add(new { RoleId = TechnicianRoleId, PermissionId = permission.Id });
            }
        }

        return rows.ToArray();
    }

    private static Guid BuildPermissionId(SystemModule module, PermissionAction action)
    {
        Span<byte> bytes = stackalloc byte[16];
        BitConverter.TryWriteBytes(bytes, (int)module);
        BitConverter.TryWriteBytes(bytes[4..], (int)action);
        return new Guid(bytes);
    }

    private sealed record PermissionSeed(Guid Id, SystemModule Module, PermissionAction Action, string Name, DateTime CreatedAt);
}
