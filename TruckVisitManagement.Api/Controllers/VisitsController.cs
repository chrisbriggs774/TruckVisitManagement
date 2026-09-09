using Microsoft.AspNetCore.Mvc;
using TruckVisitManagement.Api.Contracts.Requests;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.Mapping;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Commands.CreateVisit;
using TruckVisitManagement.Application.Commands.UpdateVisitStatus;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Application.Queries.GetVisitById;
using TruckVisitManagement.Application.Queries.SearchVisits;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.Controllers;

[ApiController]
[Route("api/visits")]
[Produces("application/json")]
public sealed class VisitsController : ControllerBase
{
    private readonly ICommandHandler<CreateVisitCommand, Visit> _createVisitHandler;
    private readonly ICommandHandler<UpdateVisitStatusCommand, Visit> _updateVisitStatusHandler;
    private readonly IQueryHandler<GetVisitByIdQuery, Visit?> _getVisitByIdHandler;
    private readonly IQueryHandler<SearchVisitsQuery, VisitSearchResult> _searchVisitsHandler;

    public VisitsController(
        ICommandHandler<CreateVisitCommand, Visit> createVisitHandler,
        ICommandHandler<UpdateVisitStatusCommand, Visit> updateVisitStatusHandler,
        IQueryHandler<GetVisitByIdQuery, Visit?> getVisitByIdHandler,
        IQueryHandler<SearchVisitsQuery, VisitSearchResult> searchVisitsHandler)
    {
        _createVisitHandler = createVisitHandler;
        _updateVisitStatusHandler = updateVisitStatusHandler;
        _getVisitByIdHandler = getVisitByIdHandler;
        _searchVisitsHandler = searchVisitsHandler;
    }

    /// <summary>Creates (pre-registers) a new truck visit.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VisitResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VisitResponseDto>> CreateVisit(
        [FromBody] CreateVisitRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVisitCommand(
            request.TerminalId,
            new CreateVisitTruck(request.Truck.LicensePlate, request.Truck.UnitNumber),
            request.ExternalDriverId,
            request.Collections.Select(c => new CreateVisitMovement(c.Reference, c.Description)).ToList(),
            request.Deliveries.Select(d => new CreateVisitMovement(d.Reference, d.Description)).ToList(),
            request.Trailer is null ? null : new CreateVisitTrailer(request.Trailer.Number, request.Trailer.Registration));

        var visit = await _createVisitHandler.HandleAsync(command, cancellationToken);
        var response = visit.ToResponseDto();

        return CreatedAtAction(nameof(GetVisitById), new { id = response.Id }, response);
    }

    /// <summary>Retrieves a visit by its identifier.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VisitResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitResponseDto>> GetVisitById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var visit = await _getVisitByIdHandler.HandleAsync(new GetVisitByIdQuery(id), cancellationToken);
        if (visit is null)
        {
            return NotFound();
        }

        return Ok(visit.ToResponseDto());
    }

    /// <summary>Searches visits using the documented filter, paging and sorting parameters.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<VisitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<VisitResponseDto>>> SearchVisits(
        [FromQuery] SearchVisitsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new SearchVisitsQuery(
            request.TerminalId,
            request.CurrentStatus,
            request.MovementFrom,
            request.MovementTo,
            request.CreatedTimeFrom,
            request.CreatedTimeTo,
            request.CreatedBy,
            request.Page,
            request.PageSize);

        var result = await _searchVisitsHandler.HandleAsync(query, cancellationToken);

        var response = new PagedResponseDto<VisitResponseDto>
        {
            Items = result.Items.Select(v => v.ToResponseDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };

        return Ok(response);
    }

    /// <summary>Marks that the visit has arrived at the gate.</summary>
    [HttpPost("{id:guid}/arrive-at-gate")]
    [ProducesResponseType(typeof(VisitResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult<VisitResponseDto>> ArriveAtGate(
        Guid id,
        [FromBody] VisitLifecycleActionRequestDto request,
        CancellationToken cancellationToken)
        => TransitionVisit(id, VisitStatus.AtGate, request, cancellationToken);

    /// <summary>Marks that the visit has entered the site.</summary>
    [HttpPost("{id:guid}/enter-site")]
    [ProducesResponseType(typeof(VisitResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult<VisitResponseDto>> EnterSite(
        Guid id,
        [FromBody] VisitLifecycleActionRequestDto request,
        CancellationToken cancellationToken)
        => TransitionVisit(id, VisitStatus.OnSite, request, cancellationToken);

    /// <summary>Marks that the visit has been completed.</summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(VisitResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult<VisitResponseDto>> CompleteVisit(
        Guid id,
        [FromBody] VisitLifecycleActionRequestDto request,
        CancellationToken cancellationToken)
        => TransitionVisit(id, VisitStatus.Completed, request, cancellationToken);

    private async Task<ActionResult<VisitResponseDto>> TransitionVisit(
        Guid id,
        VisitStatus targetStatus,
        VisitLifecycleActionRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateVisitStatusCommand(id, targetStatus, request.OriginatingActor);
        var visit = await _updateVisitStatusHandler.HandleAsync(command, cancellationToken);

        return Ok(visit.ToResponseDto());
    }
}
