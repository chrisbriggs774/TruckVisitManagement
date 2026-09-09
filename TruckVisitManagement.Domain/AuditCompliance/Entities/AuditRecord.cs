using TruckVisitManagement.Domain.Common;

namespace TruckVisitManagement.Domain.AuditCompliance.Entities;

public sealed class AuditRecord : Entity<Guid>
{
    public string EventType { get; }
    public string Payload { get; }
    public string OriginatingActor { get; }
    public DateTimeOffset OccurredAtUtc { get; }

    public AuditRecord(Guid id, string eventType, string payload, string originatingActor, DateTimeOffset occurredAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Event type is required.", nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(originatingActor))
        {
            throw new ArgumentException("Originating actor is required.", nameof(originatingActor));
        }

        EventType = eventType.Trim();
        Payload = payload ?? string.Empty;
        OriginatingActor = originatingActor.Trim();
        OccurredAtUtc = occurredAtUtc;
    }
}
