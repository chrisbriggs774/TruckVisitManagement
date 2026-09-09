using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.TerminalAccess.Entities;

public sealed class Permission : Entity<Guid>
{
    public string Name { get; }

    public Permission(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Permission name is required.", nameof(name));
        }

        Name = name.Trim();
    }
}
