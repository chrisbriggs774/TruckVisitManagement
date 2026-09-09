using FluentAssertions;
using NSubstitute;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Application.Queries.GetVisitById;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Queries.Tests.GetVisitById;

public class GetVisitByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnVisitFromStore()
    {
        var visit = CreateVisit();
        var store = Substitute.For<IVisitReadStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(visit);
        var handler = new GetVisitByIdQueryHandler(store);

        var result = await handler.HandleAsync(new GetVisitByIdQuery(visit.Id.Value));

        result.Should().BeSameAs(visit);
        await store.Received(1).GetByIdAsync(Arg.Is<VisitId>(id => id.Value == visit.Id.Value), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenMissing()
    {
        var store = Substitute.For<IVisitReadStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns((Visit?)null);
        var handler = new GetVisitByIdQueryHandler(store);

        var result = await handler.HandleAsync(new GetVisitByIdQuery(Guid.NewGuid()));

        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenQueryIsNull()
    {
        var store = Substitute.For<IVisitReadStore>();
        var handler = new GetVisitByIdQueryHandler(store);

        var act = async () => await handler.HandleAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
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
