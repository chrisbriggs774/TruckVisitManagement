using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Application.Commands.UpdateVisitStatus;

/// <summary>
/// Command to transition a visit to a new status. All transitions are retained
/// as an immutable audit history on the aggregate (see ADR-004).
/// Returns the updated <see cref="Visit"/> domain aggregate.
/// </summary>
public sealed record UpdateVisitStatusCommand(
    Guid VisitId,
    VisitStatus TargetStatus,
    string OriginatingActor) : ICommand<Visit>;
