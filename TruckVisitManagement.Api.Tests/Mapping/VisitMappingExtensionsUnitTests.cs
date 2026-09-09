using FluentAssertions;
using TruckVisitManagement.Api.Mapping;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Api.Tests.Mapping;

public class VisitMappingExtensionsUnitTests
{
    [Fact]
    public void ToResponseDto_ShouldMapAggregateData()
    {
        var visit = new Visit(
            VisitId.New(),
            new TerminalId("term-1"),
            new Truck(Guid.NewGuid(), new LicensePlate("aa11aaa"), new TruckUnitNumber("unit1")),
            new DriverReference(Guid.NewGuid(), "driver-1"),
            [new Collection(Guid.NewGuid(), "col-1", "Collection")],
            [new Delivery(Guid.NewGuid(), "del-1", "Delivery")],
            new Trailer(Guid.NewGuid(), new TrailerNumber("tr-1"), new TrailerRegistration("reg-1")));

        visit.ArriveAtGate("gate-1");
        visit.EnterSite("gate-1");

        var dto = visit.ToResponseDto();

        dto.Id.Should().Be(visit.Id.Value);
        dto.TerminalId.Should().Be("TERM-1");
        dto.Truck.LicensePlate.Should().Be("AA11AAA");
        dto.Truck.UnitNumber.Should().Be("UNIT1");
        dto.Trailer.Should().NotBeNull();
        dto.Driver.ExternalDriverId.Should().Be("driver-1");
        dto.Collections.Should().ContainSingle();
        dto.Deliveries.Should().ContainSingle();
        dto.StatusHistory.Should().HaveCount(2);
        dto.StatusHistory.First().PreviousStatus.Should().Be(visit.StatusHistory.First().PreviousStatus);
    }
}
