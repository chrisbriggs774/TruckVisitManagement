using TruckVisitManagement.Domain.Common;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.VisitManagement.Aggregates;

public sealed class Visit : AggregateRoot<VisitId>
{
    private readonly List<Collection> _collections = new();
    private readonly List<Delivery> _deliveries = new();
    private readonly List<VisitStatusTransition> _statusHistory = new();

    public TerminalId TerminalId { get; }
    public Truck Truck { get; private set; }
    public Trailer? Trailer { get; private set; }
    public DriverReference DriverReference { get; private set; }
    public VisitStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset? AtGateAtUtc { get; private set; }
    public DateTimeOffset? OnSiteAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public IReadOnlyCollection<Collection> Collections => _collections.AsReadOnly();
    public IReadOnlyCollection<Delivery> Deliveries => _deliveries.AsReadOnly();
    public IReadOnlyCollection<VisitStatusTransition> StatusHistory => _statusHistory.AsReadOnly();

    public Visit(
        VisitId id,
        TerminalId terminalId,
        Truck truck,
        DriverReference driverReference,
        IEnumerable<Collection>? collections = null,
        IEnumerable<Delivery>? deliveries = null,
        Trailer? trailer = null) : base(id)
    {
        ArgumentNullException.ThrowIfNull(terminalId);
        ArgumentNullException.ThrowIfNull(truck);
        ArgumentNullException.ThrowIfNull(driverReference);

        TerminalId = terminalId;
        Truck = truck;
        DriverReference = driverReference;
        Trailer = trailer;
        Status = VisitStatus.PreRegistered;
        CreatedAtUtc = DateTimeOffset.UtcNow;

        if (collections is not null)
        {
            _collections.AddRange(collections);
        }

        if (deliveries is not null)
        {
            _deliveries.AddRange(deliveries);
        }
    }

    public void AssignTrailer(Trailer trailer)
    {
        ArgumentNullException.ThrowIfNull(trailer);
        Trailer = trailer;
    }

    public void UpdateTruck(Truck truck)
    {
        ArgumentNullException.ThrowIfNull(truck);
        Truck = truck;
    }

    public void UpdateDriverReference(DriverReference driverReference)
    {
        ArgumentNullException.ThrowIfNull(driverReference);
        DriverReference = driverReference;
    }

    public void AddCollection(Collection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        _collections.Add(collection);
    }

    public void AddDelivery(Delivery delivery)
    {
        ArgumentNullException.ThrowIfNull(delivery);
        _deliveries.Add(delivery);
    }

    public void ArriveAtGate(string originatingActor)
    {
        TransitionTo(VisitStatus.PreRegistered, VisitStatus.AtGate, originatingActor);
        AtGateAtUtc = DateTimeOffset.UtcNow;
    }

    public void EnterSite(string originatingActor)
    {
        TransitionTo(VisitStatus.AtGate, VisitStatus.OnSite, originatingActor);
        OnSiteAtUtc = DateTimeOffset.UtcNow;
    }

    public void CheckOut(string originatingActor)
    {
        TransitionTo(VisitStatus.OnSite, VisitStatus.Completed, originatingActor);
        CompletedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Cancel(string originatingActor)
    {
        if (Status is VisitStatus.Completed)
        {
            throw new InvalidOperationException("Completed visits cannot be cancelled.");
        }

        if (string.IsNullOrWhiteSpace(originatingActor))
        {
            throw new ArgumentException("Originating actor is required.", nameof(originatingActor));
        }

        var changedAtUtc = DateTimeOffset.UtcNow;
        var previousStatus = Status;
        Status = VisitStatus.Cancelled;

        _statusHistory.Add(new VisitStatusTransition(
            Guid.NewGuid(),
            previousStatus,
            VisitStatus.Cancelled,
            originatingActor,
            changedAtUtc));
    }

    public void CheckIn(string originatingActor)
    {
        ArriveAtGate(originatingActor);
    }

    public void StartOperations(string originatingActor)
    {
        EnterSite(originatingActor);
    }

    public void Complete(string originatingActor)
    {
        CheckOut(originatingActor);
    }

    private void TransitionTo(VisitStatus requiredCurrentStatus, VisitStatus newStatus, string originatingActor)
    {
        if (Status != requiredCurrentStatus)
        {
            throw new InvalidOperationException($"Invalid state transition from {Status} to {newStatus}.");
        }

        if (string.IsNullOrWhiteSpace(originatingActor))
        {
            throw new ArgumentException("Originating actor is required.", nameof(originatingActor));
        }

        var changedAtUtc = DateTimeOffset.UtcNow;
        Status = newStatus;

        _statusHistory.Add(new VisitStatusTransition(
            Guid.NewGuid(),
            requiredCurrentStatus,
            newStatus,
            originatingActor,
            changedAtUtc));
    }
}
