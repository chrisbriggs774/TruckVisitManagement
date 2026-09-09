using FluentAssertions;
using NSubstitute;
using TruckVisitManagement.Application.Commands;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Commands.UpdateVisitStatus;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Commands.Tests.UpdateVisitStatus;

public class UpdateVisitStatusCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldTransitionVisitAndPersistUpdate()
    {
        var visit = CreateVisit();
        var store = Substitute.For<IVisitWriteStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(visit);
        store.UpdateAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var handler = new UpdateVisitStatusCommandHandler(store);

        var result = await handler.HandleAsync(new UpdateVisitStatusCommand(visit.Id.Value, VisitStatus.AtGate, "gate-1"));

        result.Should().BeSameAs(visit);
        result.Status.Should().Be(VisitStatus.AtGate);
        visit.StatusHistory.Should().ContainSingle();
        await store.Received(1).UpdateAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowVisitNotFound_WhenMissing()
    {
        var store = Substitute.For<IVisitWriteStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns((Visit?)null);
        var handler = new UpdateVisitStatusCommandHandler(store);

        var act = async () => await handler.HandleAsync(new UpdateVisitStatusCommand(Guid.NewGuid(), VisitStatus.AtGate, "gate-1"));

        await act.Should().ThrowAsync<VisitNotFoundException>();
        await store.DidNotReceive().UpdateAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectRegressionToPreRegistered()
    {
        var visit = CreateVisit();
        visit.ArriveAtGate("actor");
        var store = Substitute.For<IVisitWriteStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(visit);
        var handler = new UpdateVisitStatusCommandHandler(store);

        var act = async () => await handler.HandleAsync(new UpdateVisitStatusCommand(visit.Id.Value, VisitStatus.PreRegistered, "gate-1"));

        await act.Should().ThrowAsync<InvalidVisitStatusTransitionException>();
        await store.DidNotReceive().UpdateAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldSupportCancellationTransition()
    {
        var visit = CreateVisit();
        var store = Substitute.For<IVisitWriteStore>();
        store.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(visit);
        store.UpdateAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var handler = new UpdateVisitStatusCommandHandler(store);

        var result = await handler.HandleAsync(new UpdateVisitStatusCommand(visit.Id.Value, VisitStatus.Cancelled, "gate-1"));

        result.Status.Should().Be(VisitStatus.Cancelled);
        result.StatusHistory.Should().ContainSingle();
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenCommandIsNull()
    {
        var store = Substitute.For<IVisitWriteStore>();
        var handler = new UpdateVisitStatusCommandHandler(store);

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
