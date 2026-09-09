using FluentAssertions;
using NSubstitute;
using TruckVisitManagement.Application.Commands.CreateVisit;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Application.Commands.Tests.CreateVisit;

public class CreateVisitCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateNormalizedVisitAndPersistIt()
    {
        var store = Substitute.For<IVisitWriteStore>();
        Visit? persisted = null;
        store.AddAsync(Arg.Do<Visit>(v => persisted = v), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new CreateVisitCommandHandler(store);
        var command = new CreateVisitCommand(
            " term-1 ",
            new CreateVisitTruck("aa 11 aaa", " unit 1 "),
            " driver-1 ",
            [new CreateVisitMovement(" col-1 ", "collection")],
            [new CreateVisitMovement(" del-1 ", "delivery")],
            new CreateVisitTrailer(" tr 1 ", " reg 1 "));

        var visit = await handler.HandleAsync(command);

        persisted.Should().NotBeNull();
        persisted!.Should().BeSameAs(visit);
        visit.TerminalId.Value.Should().Be("TERM-1");
        visit.Status.Should().Be(VisitStatus.PreRegistered);
        visit.Truck.LicensePlate.Value.Should().Be("AA11AAA");
        visit.Truck.UnitNumber.Value.Should().Be("UNIT1");
        visit.DriverReference.ExternalDriverId.Should().Be("driver-1");
        visit.Trailer.Should().NotBeNull();
        visit.Trailer!.Number.Value.Should().Be("TR1");
        visit.Trailer.Registration.Value.Should().Be("REG1");
        visit.Collections.Should().ContainSingle();
        visit.Deliveries.Should().ContainSingle();
        await store.Received(1).AddAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldAllowNullTrailerAndEmptyMovements()
    {
        var store = Substitute.For<IVisitWriteStore>();
        store.AddAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        var handler = new CreateVisitCommandHandler(store);

        var visit = await handler.HandleAsync(new CreateVisitCommand(
            "TERM-1",
            new CreateVisitTruck("AA11AAA", "UNIT1"),
            "driver-1",
            [],
            [],
            null));

        visit.Trailer.Should().BeNull();
        visit.Collections.Should().BeEmpty();
        visit.Deliveries.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenCommandIsNull()
    {
        var store = Substitute.For<IVisitWriteStore>();
        var handler = new CreateVisitCommandHandler(store);

        var act = async () => await handler.HandleAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenWriteStoreIsNull()
    {
        var act = () => new CreateVisitCommandHandler(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
