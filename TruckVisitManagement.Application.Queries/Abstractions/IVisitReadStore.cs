using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Enums;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Queries.Abstractions;

/// <summary>
/// Read-side query abstraction for visits.
/// Per ADR-001 this is backed by the read-optimised query store (OpenSearch in production).
/// </summary>
public interface IVisitReadStore
{
    Task<Visit?> GetByIdAsync(VisitId visitId, CancellationToken cancellationToken = default);

    Task<VisitSearchResult> SearchAsync(VisitSearchCriteria criteria, CancellationToken cancellationToken = default);
}

/// <summary>
/// Filtering, sorting and paging criteria for searching visits.
/// Mirrors the documented query parameters in the README.
/// </summary>
public sealed record VisitSearchCriteria(
    string? TerminalId,
    VisitStatus? CurrentStatus,
    DateTimeOffset? MovementFrom,
    DateTimeOffset? MovementTo,
    DateTimeOffset? CreatedTimeFrom,
    DateTimeOffset? CreatedTimeTo,
    string? CreatedBy,
    int Page,
    int PageSize);

/// <summary>
/// A page of matching visits together with paging metadata.
/// </summary>
public sealed record VisitSearchResult(
    IReadOnlyCollection<Visit> Items,
    int Page,
    int PageSize,
    long TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
