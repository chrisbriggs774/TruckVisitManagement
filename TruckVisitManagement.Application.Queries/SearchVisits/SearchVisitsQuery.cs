using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Application.Queries.SearchVisits;

/// <summary>
/// Query to search visits using the documented filter, paging and sorting parameters.
/// Returns a <see cref="VisitSearchResult"/> containing domain aggregates and paging metadata.
/// </summary>
public sealed record SearchVisitsQuery(
    string? TerminalId,
    VisitStatus? CurrentStatus,
    DateTimeOffset? MovementFrom,
    DateTimeOffset? MovementTo,
    DateTimeOffset? CreatedTimeFrom,
    DateTimeOffset? CreatedTimeTo,
    string? CreatedBy,
    int Page = 1,
    int PageSize = 25) : IQuery<VisitSearchResult>;
