using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.AuditCompliance.Entities;

public sealed class StatusHistory : Entity<Guid>
{
    public VisitId VisitId { get; }
    public VisitStatus PreviousStatus { get; }
    public VisitStatus NewStatus { get; }
    public string OriginatingActor { get; }
    public DateTimeOffset ChangedAtUtc { get; }

    public StatusHistory(
        Guid id,
        VisitId visitId,
        VisitStatus previousStatus,
        VisitStatus newStatus,
        string originatingActor,
        DateTimeOffset changedAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(originatingActor))
        {
            throw new ArgumentException("Originating actor is required.", nameof(originatingActor));
        }

        VisitId = visitId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        OriginatingActor = originatingActor.Trim();
        ChangedAtUtc = changedAtUtc;
    }
}
