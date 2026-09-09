using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TruckVisitManagement.Api.Contracts.Requests;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.Controllers;
using TruckVisitManagement.Api.Mapping;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Commands.CreateVisit;
using TruckVisitManagement.Application.Commands.UpdateVisitStatus;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Application.Queries.GetVisitById;
using TruckVisitManagement.Application.Queries.SearchVisits;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Api.Tests.Controllers;

public class VisitsControllerUnitTests
{
    [Fact]
    public async Task CreateVisit_ShouldMapRequestAndReturnCreatedResult()
    {
        var visit = CreateVisitAggregate();
        CreateVisitCommand? receivedCommand = null;

        var controller = CreateController(
            createResult: visit,
            createCommandCapture: c => receivedCommand = c);

        var request = new CreateVisitRequestDto
        {
            TerminalId = "TERM-1",
            ExternalDriverId = "driver-1",
            Truck = new CreateVisitTruckDto { LicensePlate = "aa11aaa", UnitNumber = "unit 1" },
            Trailer = new CreateVisitTrailerDto { Number = "tr 1", Registration = "reg 1" },
            Collections = [new CreateVisitMovementDto { Reference = "col-1", Description = "Collection" }],
            Deliveries = [new CreateVisitMovementDto { Reference = "del-1", Description = "Delivery" }]
        };

        var result = await controller.CreateVisit(request, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result.As<CreatedAtActionResult>();
        created!.ActionName.Should().Be(nameof(VisitsController.GetVisitById));
        created.RouteValues!["id"].Should().Be(visit.Id.Value);
        created.Value.Should().BeAssignableTo<VisitResponseDto>();

        receivedCommand.Should().NotBeNull();
        receivedCommand!.TerminalId.Should().Be(request.TerminalId);
        receivedCommand.ExternalDriverId.Should().Be(request.ExternalDriverId);
        receivedCommand.Truck.LicensePlate.Should().Be(request.Truck.LicensePlate);
        receivedCommand.Truck.UnitNumber.Should().Be(request.Truck.UnitNumber);
        receivedCommand.Collections.Should().ContainSingle();
        receivedCommand.Deliveries.Should().ContainSingle();
        receivedCommand.Trailer.Should().NotBeNull();
    }

    [Fact]
    public async Task GetVisitById_ShouldReturnNotFound_WhenMissing()
    {
        var controller = CreateController();

        var result = await controller.GetVisitById(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetVisitById_ShouldReturnOk_WhenVisitExists()
    {
        var visit = CreateVisitAggregate();
        var controller = CreateController(getByIdResult: visit);

        var result = await controller.GetVisitById(visit.Id.Value, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(visit.ToResponseDto());
    }

    [Fact]
    public async Task SearchVisits_ShouldReturnPagedResponse()
    {
        var visit = CreateVisitAggregate();
        var searchResult = new VisitSearchResult([visit], 2, 10, 11);
        var controller = CreateController(searchResult: searchResult);

        var response = await controller.SearchVisits(new SearchVisitsRequestDto { Page = 2, PageSize = 10 }, CancellationToken.None);

        var ok = response.Result.Should().BeOfType<OkObjectResult>().Subject;
        var page = ok.Value.Should().BeOfType<PagedResponseDto<VisitResponseDto>>().Subject;
        page.Items.Should().ContainSingle();
        page.Page.Should().Be(2);
        page.PageSize.Should().Be(10);
        page.TotalCount.Should().Be(11);
        page.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task ArriveAtGate_ShouldCallUpdateHandlerWithAtGateStatus()
    {
        var visit = CreateVisitAggregate();
        UpdateVisitStatusCommand? received = null;
        var controller = CreateController(updateResult: visit, updateCommandCapture: c => received = c);

        var result = await controller.ArriveAtGate(Guid.NewGuid(), new VisitLifecycleActionRequestDto { OriginatingActor = "gate-1" }, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        received.Should().NotBeNull();
        received!.TargetStatus.Should().Be(VisitStatus.AtGate);
        received.OriginatingActor.Should().Be("gate-1");
    }

    [Fact]
    public async Task EnterSite_ShouldCallUpdateHandlerWithOnSiteStatus()
    {
        var visit = CreateVisitAggregate();
        UpdateVisitStatusCommand? received = null;
        var controller = CreateController(updateResult: visit, updateCommandCapture: c => received = c);

        var result = await controller.EnterSite(Guid.NewGuid(), new VisitLifecycleActionRequestDto { OriginatingActor = "gate-1" }, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        received.Should().NotBeNull();
        received!.TargetStatus.Should().Be(VisitStatus.OnSite);
        received.OriginatingActor.Should().Be("gate-1");
    }

    [Fact]
    public async Task CompleteVisit_ShouldCallUpdateHandlerWithCompletedStatus()
    {
        var visit = CreateVisitAggregate();
        UpdateVisitStatusCommand? received = null;
        var controller = CreateController(updateResult: visit, updateCommandCapture: c => received = c);

        var result = await controller.CompleteVisit(Guid.NewGuid(), new VisitLifecycleActionRequestDto { OriginatingActor = "gate-1" }, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        received.Should().NotBeNull();
        received!.TargetStatus.Should().Be(VisitStatus.Completed);
        received.OriginatingActor.Should().Be("gate-1");
    }

    private static VisitsController CreateController(
        Visit? createResult = null,
        Visit? getByIdResult = null,
        VisitSearchResult? searchResult = null,
        Visit? updateResult = null,
        Action<CreateVisitCommand>? createCommandCapture = null,
        Action<UpdateVisitStatusCommand>? updateCommandCapture = null)
    {
        var createHandler = Substitute.For<ICommandHandler<CreateVisitCommand, Visit>>();
        var updateHandler = Substitute.For<ICommandHandler<UpdateVisitStatusCommand, Visit>>();
        var getByIdHandler = Substitute.For<IQueryHandler<GetVisitByIdQuery, Visit?>>();
        var searchHandler = Substitute.For<IQueryHandler<SearchVisitsQuery, VisitSearchResult>>();

        if (createResult is not null)
        {
            createHandler.HandleAsync(Arg.Do<CreateVisitCommand>(c => createCommandCapture?.Invoke(c)), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(createResult));
        }

        if (getByIdResult is not null)
        {
            getByIdHandler.HandleAsync(Arg.Any<GetVisitByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Visit?>(getByIdResult));
        }

        if (searchResult is not null)
        {
            searchHandler.HandleAsync(Arg.Any<SearchVisitsQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(searchResult));
        }

        if (updateResult is not null)
        {
            updateHandler.HandleAsync(Arg.Do<UpdateVisitStatusCommand>(c => updateCommandCapture?.Invoke(c)), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(updateResult));
        }

        return new VisitsController(createHandler, updateHandler, getByIdHandler, searchHandler);
    }

    private static Visit CreateVisitAggregate()
    {
        var visit = new Visit(
            VisitId.New(),
            new TerminalId("TERM-1"),
            new Truck(Guid.NewGuid(), new LicensePlate("AA11AAA"), new TruckUnitNumber("UNIT1")),
            new DriverReference(Guid.NewGuid(), "DR-1"),
            [new Collection(Guid.NewGuid(), "COL-1", "Collection")],
            [new Delivery(Guid.NewGuid(), "DEL-1", "Delivery")],
            new Trailer(Guid.NewGuid(), new TrailerNumber("TR1"), new TrailerRegistration("REG1")));

        visit.ArriveAtGate("actor");
        return visit;
    }
}
