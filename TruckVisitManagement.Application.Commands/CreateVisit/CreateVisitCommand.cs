using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;

namespace TruckVisitManagement.Application.Commands.CreateVisit;

/// <summary>
/// Command to create (pre-register) a new truck visit.
/// Returns the created <see cref="Visit"/> domain aggregate.
/// </summary>
public sealed record CreateVisitCommand(
    string TerminalId,
    CreateVisitTruck Truck,
    string ExternalDriverId,
    IReadOnlyCollection<CreateVisitMovement> Collections,
    IReadOnlyCollection<CreateVisitMovement> Deliveries,
    CreateVisitTrailer? Trailer = null) : ICommand<Visit>;