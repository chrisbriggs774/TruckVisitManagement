using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Domain.Tests.VisitManagement;

public class VisitEntitiesDomainTests
{
    [Fact]
    public void DriverReference_ShouldTrimExternalDriverId()
    {
        var entity = new DriverReference(Guid.NewGuid(), "  DR-1  ");

        Assert.Equal("DR-1", entity.ExternalDriverId);
    }

    [Fact]
    public void DriverReference_ShouldRejectBlankExternalDriverId()
    {
        Assert.Throws<ArgumentException>(() => new DriverReference(Guid.NewGuid(), " "));
    }

    [Fact]
    public void Collection_ShouldNormalizeReference_AndTrimDescription()
    {
        var entity = new Collection(Guid.NewGuid(), " col-1 ", "  collect docs  ");

        Assert.Equal("COL-1", entity.Reference);
        Assert.Equal("collect docs", entity.Description);
    }

    [Fact]
    public void Collection_ShouldUpdateDescriptionWithTrimmedValue()
    {
        var entity = new Collection(Guid.NewGuid(), "COL-1", "old");

        entity.UpdateDescription("  updated  ");

        Assert.Equal("updated", entity.Description);
    }

    [Fact]
    public void Collection_ShouldRejectBlankReference()
    {
        Assert.Throws<ArgumentException>(() => new Collection(Guid.NewGuid(), " ", "desc"));
    }

    [Fact]
    public void Delivery_ShouldNormalizeReference_AndTrimDescription()
    {
        var entity = new Delivery(Guid.NewGuid(), " del-1 ", "  drop off  ");

        Assert.Equal("DEL-1", entity.Reference);
        Assert.Equal("drop off", entity.Description);
    }

    [Fact]
    public void Delivery_ShouldRejectBlankReference()
    {
        Assert.Throws<ArgumentException>(() => new Delivery(Guid.NewGuid(), " ", "desc"));
    }

    [Fact]
    public void Truck_ShouldUpdateIdentity()
    {
        var truck = new Truck(Guid.NewGuid(), new LicensePlate("AA11AAA"), new TruckUnitNumber("UNIT1"));

        truck.UpdateIdentity(new LicensePlate("bb22bbb"), new TruckUnitNumber(" unit 2 "));

        Assert.Equal("BB22BBB", truck.LicensePlate.Value);
        Assert.Equal("UNIT2", truck.UnitNumber.Value);
    }

    [Fact]
    public void Trailer_ShouldUpdateIdentity()
    {
        var trailer = new Trailer(Guid.NewGuid(), new TrailerNumber("TR1"), new TrailerRegistration("RG1"));

        trailer.UpdateIdentity(new TrailerNumber(" tr 2 "), new TrailerRegistration(" rg 2 "));

        Assert.Equal("TR2", trailer.Number.Value);
        Assert.Equal("RG2", trailer.Registration.Value);
    }

    [Fact]
    public void VisitStatusTransition_ShouldCaptureTransitionData()
    {
        var changedAt = DateTimeOffset.UtcNow;
        var transition = new VisitStatusTransition(
            Guid.NewGuid(),
            VisitStatus.PreRegistered,
            VisitStatus.AtGate,
            "  gate-system  ",
            changedAt);

        Assert.Equal(VisitStatus.PreRegistered, transition.PreviousStatus);
        Assert.Equal(VisitStatus.AtGate, transition.NewStatus);
        Assert.Equal("gate-system", transition.OriginatingActor);
        Assert.Equal(changedAt, transition.ChangedAtUtc);
    }

    [Fact]
    public void VisitStatusTransition_ShouldRejectBlankActor()
    {
        Assert.Throws<ArgumentException>(() => new VisitStatusTransition(
            Guid.NewGuid(),
            VisitStatus.PreRegistered,
            VisitStatus.AtGate,
            " ",
            DateTimeOffset.UtcNow));
    }
}
