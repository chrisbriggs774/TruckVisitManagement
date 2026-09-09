using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.TerminalAccess.Entities;

public sealed class Role : Entity<Guid>
{
    private readonly HashSet<Guid> _permissionIds = new();

    public string Name { get; }
    public IReadOnlyCollection<Guid> PermissionIds => _permissionIds;

    public Role(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        Name = name.Trim();
    }

    public void AddPermission(Permission permission)
    {
        _permissionIds.Add(permission.Id);
    }
}
