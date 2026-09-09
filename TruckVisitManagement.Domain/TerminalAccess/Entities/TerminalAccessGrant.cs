using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.TerminalAccess.Entities;

public sealed class TerminalAccessGrant : Entity<Guid>
{
    private readonly HashSet<Guid> _roleIds = new();

    public Guid UserId { get; }
    public TerminalId TerminalId { get; }
    public IReadOnlyCollection<Guid> RoleIds => _roleIds;

    public TerminalAccessGrant(Guid id, Guid userId, TerminalId terminalId) : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        UserId = userId;
        TerminalId = terminalId;
    }

    public void AddRole(Role role)
    {
        _roleIds.Add(role.Id);
    }
}
