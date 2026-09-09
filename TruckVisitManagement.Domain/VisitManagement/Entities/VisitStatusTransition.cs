using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Domain.VisitManagement.Entities;

public sealed class VisitStatusTransition : Entity<Guid>
{
    public VisitStatus PreviousStatus { get; }
    public VisitStatus NewStatus { get; }
    public DateTimeOffset ChangedAtUtc { get; }
    public string OriginatingActor { get; }

    public VisitStatusTransition(
        Guid id,
        VisitStatus previousStatus,
        VisitStatus newStatus,
        string originatingActor,
        DateTimeOffset changedAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(originatingActor))
        {
            throw new ArgumentException("Originating actor is required.", nameof(originatingActor));
        }

        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        OriginatingActor = originatingActor.Trim();
        ChangedAtUtc = changedAtUtc;
    }
}
