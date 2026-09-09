using TruckVisitManagement.Domain.AuditCompliance.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.Tests.AuditCompliance;

public class AuditComplianceDomainTests
{
    [Fact]
    public void AuditRecord_ShouldTrimFields_AndAllowEmptyPayload()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var record = new AuditRecord(Guid.NewGuid(), "  VisitStatusChanged  ", null!, "  system  ", occurredAt);

        Assert.Equal("VisitStatusChanged", record.EventType);
        Assert.Equal(string.Empty, record.Payload);
        Assert.Equal("system", record.OriginatingActor);
        Assert.Equal(occurredAt, record.OccurredAtUtc);
    }

    [Fact]
    public void AuditRecord_ShouldRejectBlankEventType()
    {
        Assert.Throws<ArgumentException>(() => new AuditRecord(Guid.NewGuid(), " ", "{}", "actor", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AuditRecord_ShouldRejectBlankActor()
    {
        Assert.Throws<ArgumentException>(() => new AuditRecord(Guid.NewGuid(), "VisitCreated", "{}", " ", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void StatusHistory_ShouldCaptureAuditTransition()
    {
        var changedAt = DateTimeOffset.UtcNow;
        var history = new StatusHistory(
            Guid.NewGuid(),
            VisitId.New(),
            VisitStatus.PreRegistered,
            VisitStatus.AtGate,
            "  gate  ",
            changedAt);

        Assert.Equal(VisitStatus.PreRegistered, history.PreviousStatus);
        Assert.Equal(VisitStatus.AtGate, history.NewStatus);
        Assert.Equal("gate", history.OriginatingActor);
        Assert.Equal(changedAt, history.ChangedAtUtc);
    }

    [Fact]
    public void StatusHistory_ShouldRejectBlankActor()
    {
        Assert.Throws<ArgumentException>(() => new StatusHistory(
            Guid.NewGuid(),
            VisitId.New(),
            VisitStatus.PreRegistered,
            VisitStatus.AtGate,
            " ",
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AuditTrail_ShouldAppendRecordsInOrder()
    {
        var trail = new AuditTrail(Guid.NewGuid());
        var first = new AuditRecord(Guid.NewGuid(), "VisitCreated", "{}", "system", DateTimeOffset.UtcNow);
        var second = new AuditRecord(Guid.NewGuid(), "VisitStatusChanged", "{}", "system", DateTimeOffset.UtcNow);

        trail.Append(first);
        trail.Append(second);

        Assert.Equal(2, trail.Records.Count);
        Assert.Same(first, trail.Records.First());
        Assert.Same(second, trail.Records.Last());
    }
}
