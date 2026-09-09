using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;

namespace TruckVisitManagement.Application.Queries.GetVisitById;

/// <summary>
/// Query to retrieve a single visit by its identifier.
/// Returns the <see cref="Visit"/> domain aggregate, or <c>null</c> when not found.
/// </summary>
public sealed record GetVisitByIdQuery(Guid VisitId) : IQuery<Visit?>;
