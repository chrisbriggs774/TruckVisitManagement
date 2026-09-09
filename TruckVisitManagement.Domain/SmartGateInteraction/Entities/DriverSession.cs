using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.SmartGateInteraction.Enums;

namespace TruckVisitManagement.Domain.SmartGateInteraction.Entities;

public sealed class DriverSession : Entity<Guid>
{
    public Guid DriverIdentityId { get; }
    public SessionStatus Status { get; private set; }

    public DriverSession(Guid id, Guid driverIdentityId) : base(id)
    {
        if (driverIdentityId == Guid.Empty)
        {
            throw new ArgumentException("Driver identity id cannot be empty.", nameof(driverIdentityId));
        }

        DriverIdentityId = driverIdentityId;
        Status = SessionStatus.Active;
    }

    public void Complete()
    {
        Status = SessionStatus.Completed;
    }

    public void Escalate()
    {
        Status = SessionStatus.Escalated;
    }
}
