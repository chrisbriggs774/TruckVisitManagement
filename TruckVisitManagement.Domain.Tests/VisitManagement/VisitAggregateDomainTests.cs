using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.Tests.VisitManagement;

public class VisitAggregateDomainTests
{
    [Fact]
    public void NewVisit_ShouldStartPreRegistered_WithNoStatusHistory()
    {
        var visit = CreateVisit();

        Assert.Equal(VisitStatus.PreRegistered, visit.Status);
        Assert.Empty(visit.StatusHistory);
        Assert.True((DateTimeOffset.UtcNow - visit.CreatedAtUtc) < TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ArriveAtGate_ShouldTransitionAndAppendHistory()
    {
        var visit = CreateVisit();

        visit.ArriveAtGate("driver-1");

        Assert.Equal(VisitStatus.AtGate, visit.Status);
        Assert.NotNull(visit.AtGateAtUtc);
        Assert.Single(visit.StatusHistory);
        Assert.Equal(VisitStatus.PreRegistered, visit.StatusHistory.Single().PreviousStatus);
        Assert.Equal(VisitStatus.AtGate, visit.StatusHistory.Single().NewStatus);
    }

    [Fact]
    public void EnterSite_ShouldRequireAtGate()
    {
        var visit = CreateVisit();

        Assert.Throws<InvalidOperationException>(() => visit.EnterSite("driver-1"));
    }

    [Fact]
    public void CheckOut_ShouldRequireOnSite()
    {
        var visit = CreateVisit();

        Assert.Throws<InvalidOperationException>(() => visit.CheckOut("driver-1"));
    }

    [Fact]
    public void LifecycleAliases_ShouldMapToSameTransitions()
    {
        var visit = CreateVisit();

        visit.CheckIn("actor");
        visit.StartOperations("actor");
        visit.Complete("actor");

        Assert.Equal(VisitStatus.Completed, visit.Status);
        Assert.Equal(3, visit.StatusHistory.Count);
    }

    [Fact]
    public void Cancel_ShouldSetCancelled_AndAppendHistory()
    {
        var visit = CreateVisit();

        visit.Cancel("gate-operator");

        Assert.Equal(VisitStatus.Cancelled, visit.Status);
        Assert.Single(visit.StatusHistory);
        Assert.Equal(VisitStatus.Cancelled, visit.StatusHistory.Single().NewStatus);
    }

    [Fact]
    public void Cancel_ShouldRejectCompletedVisit()
    {
        var visit = CreateVisit();
        visit.ArriveAtGate("actor");
        visit.EnterSite("actor");
        visit.CheckOut("actor");

        Assert.Throws<InvalidOperationException>(() => visit.Cancel("actor"));
    }

    [Fact]
    public void Transitions_ShouldRejectBlankActor()
    {
        var visit = CreateVisit();

        Assert.Throws<ArgumentException>(() => visit.ArriveAtGate(" "));
    }

    [Fact]
    public void Aggregate_ShouldSupportMutatingAssociatedData()
    {
        var visit = CreateVisit();
        var newTruck = new Truck(Guid.NewGuid(), new LicensePlate("CC33CCC"), new TruckUnitNumber("UNIT3"));
        var newDriver = new DriverReference(Guid.NewGuid(), "DR-2");
        var trailer = new Trailer(Guid.NewGuid(), new TrailerNumber("TR2"), new TrailerRegistration("REG2"));

        visit.UpdateTruck(newTruck);
        visit.UpdateDriverReference(newDriver);
        visit.AssignTrailer(trailer);
        visit.AddCollection(new Collection(Guid.NewGuid(), "COL-2", "Collection 2"));
        visit.AddDelivery(new Delivery(Guid.NewGuid(), "DEL-2", "Delivery 2"));

        Assert.Same(newTruck, visit.Truck);
        Assert.Same(newDriver, visit.DriverReference);
        Assert.Same(trailer, visit.Trailer);
        Assert.Single(visit.Collections.Where(c => c.Reference == "COL-2"));
        Assert.Single(visit.Deliveries.Where(d => d.Reference == "DEL-2"));
    }

    [Fact]
    public void History_ShouldBeAppendOnly_AcrossValidTransitions()
    {
        var visit = CreateVisit();

        visit.ArriveAtGate("driver-1");
        var firstEntryId = visit.StatusHistory.Single().Id;

        visit.EnterSite("driver-1");

        Assert.Equal(2, visit.StatusHistory.Count);
        Assert.Equal(firstEntryId, visit.StatusHistory.First().Id);
    }

    private static Visit CreateVisit()
    {
        return new Visit(
            VisitId.New(),
            new TerminalId("TERM-1"),
            new Truck(Guid.NewGuid(), new LicensePlate("AA11AAA"), new TruckUnitNumber("UNIT1")),
            new DriverReference(Guid.NewGuid(), "DR-1"));
    }
}
