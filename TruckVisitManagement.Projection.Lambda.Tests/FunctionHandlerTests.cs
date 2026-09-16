using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using FluentAssertions;
using NSubstitute;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Projection.Lambda.Tests;

public class FunctionHandlerTests
{
    private readonly IVisitWriteStore _writeStore = Substitute.For<IVisitWriteStore>();
    private readonly IVisitReadStore _readStore = Substitute.For<IVisitReadStore>();
    private readonly ILambdaContext _context = Substitute.For<ILambdaContext>();

    public FunctionHandlerTests()
    {
        _context.Logger.Returns(Substitute.For<ILambdaLogger>());
    }

    [Fact]
    public async Task HandleAsync_ShouldUpsertProjection_WhenVisitExists()
    {
        // Arrange
        var visitId = Guid.NewGuid();
        var visit = CreateVisit(visitId);
        _writeStore.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(visit);
        var handler = new FunctionHandler(_writeStore, _readStore);
        var streamEvent = CreateStreamEvent(visitId, "INSERT");

        // Act
        await handler.HandleAsync(streamEvent, _context);

        // Assert
        await _readStore.Received(1).UpsertAsync(visit, Arg.Any<CancellationToken>());
        await _readStore.DidNotReceive().DeleteAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProjection_WhenEventIsRemove()
    {
        // Arrange
        var visitId = Guid.NewGuid();
        var handler = new FunctionHandler(_writeStore, _readStore);
        var streamEvent = CreateStreamEvent(visitId, "REMOVE");

        // Act
        await handler.HandleAsync(streamEvent, _context);

        // Assert
        await _readStore.Received(1).DeleteAsync(
            Arg.Is<VisitId>(id => id.Value == visitId),
            Arg.Any<CancellationToken>());
        await _writeStore.DidNotReceive().GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>());
        await _readStore.DidNotReceive().UpsertAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProjection_WhenVisitNotFoundInWriteStore()
    {
        // Arrange
        var visitId = Guid.NewGuid();
        _writeStore.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns((Visit?)null);
        var handler = new FunctionHandler(_writeStore, _readStore);
        var streamEvent = CreateStreamEvent(visitId, "MODIFY");

        // Act
        await handler.HandleAsync(streamEvent, _context);

        // Assert
        await _readStore.Received(1).DeleteAsync(
            Arg.Is<VisitId>(id => id.Value == visitId),
            Arg.Any<CancellationToken>());
        await _readStore.DidNotReceive().UpsertAsync(Arg.Any<Visit>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldProcessAllRecords_InBatch()
    {
        // Arrange
        var upsertVisitId = Guid.NewGuid();
        var removeVisitId = Guid.NewGuid();
        var upsertVisit = CreateVisit(upsertVisitId);
        _writeStore.GetByIdAsync(Arg.Any<VisitId>(), Arg.Any<CancellationToken>()).Returns(upsertVisit);
        var handler = new FunctionHandler(_writeStore, _readStore);

        var streamEvent = new DynamoDBEvent
        {
            Records =
            [
                CreateStreamRecord(upsertVisitId, "INSERT"),
                CreateStreamRecord(removeVisitId, "REMOVE")
            ]
        };

        // Act
        await handler.HandleAsync(streamEvent, _context);

        // Assert
        await _readStore.Received(1).UpsertAsync(upsertVisit, Arg.Any<CancellationToken>());
        await _readStore.Received(1).DeleteAsync(
            Arg.Is<VisitId>(id => id.Value == removeVisitId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowArgumentNullException_WhenStreamEventIsNull()
    {
        // Arrange
        var handler = new FunctionHandler(_writeStore, _readStore);

        // Act
        var act = async () => await handler.HandleAsync(null!, _context);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private static DynamoDBEvent CreateStreamEvent(Guid visitId, string eventName)
        => new()
        {
            Records = [CreateStreamRecord(visitId, eventName)]
        };

    private static DynamoDBEvent.DynamodbStreamRecord CreateStreamRecord(Guid visitId, string eventName)
        => new()
        {
            EventName = eventName,
            Dynamodb = new Amazon.DynamoDBv2.Model.StreamRecord
            {
                Keys = new Dictionary<string, AttributeValue>
                {
                    ["VisitId"] = new() { S = visitId.ToString() }
                }
            }
        };
    private static Visit CreateVisit(Guid visitId)
        => new(
            VisitId.From(visitId),
            new TerminalId("TERM-1"),
            new Truck(Guid.NewGuid(), new LicensePlate("AA11AAA"), new TruckUnitNumber("UNIT1")),
            new DriverReference(Guid.NewGuid(), "DR-1"));
}
