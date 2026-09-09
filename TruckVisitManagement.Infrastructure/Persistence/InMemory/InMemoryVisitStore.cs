using System.Collections.Concurrent;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Infrastructure.Persistence.InMemory;

/// <summary>
/// A simple in-memory store used to make the API runnable for local development.
/// This intentionally implements both the write-side and read-side abstractions so a
/// single instance backs the whole application until DynamoDB (command store) and
/// OpenSearch (query store) are introduced per ADR-001.
/// </summary>
public sealed class InMemoryVisitStore : IVisitWriteStore, IVisitReadStore
{
    private readonly ConcurrentDictionary<Guid, Visit> _visits = new();

    public Task AddAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(visit);
        _visits[visit.Id.Value] = visit;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(visit);
        _visits[visit.Id.Value] = visit;
        return Task.CompletedTask;
    }

    public Task<Visit?> GetByIdAsync(VisitId visitId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(visitId);
        _visits.TryGetValue(visitId.Value, out var visit);
        return Task.FromResult(visit);
    }

    public Task<VisitSearchResult> SearchAsync(VisitSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        IEnumerable<Visit> query = _visits.Values;

        if (!string.IsNullOrWhiteSpace(criteria.TerminalId))
        {
            var terminalId = new TerminalId(criteria.TerminalId);
            query = query.Where(v => v.TerminalId.Equals(terminalId));
        }

        if (criteria.CurrentStatus is not null)
        {
            query = query.Where(v => v.Status == criteria.CurrentStatus);
        }

        if (criteria.CreatedTimeFrom is not null)
        {
            query = query.Where(v => v.CreatedAtUtc >= criteria.CreatedTimeFrom);
        }

        if (criteria.CreatedTimeTo is not null)
        {
            query = query.Where(v => v.CreatedAtUtc <= criteria.CreatedTimeTo);
        }

        if (criteria.MovementFrom is not null)
        {
            query = query.Where(v => LatestMovementAtUtc(v) >= criteria.MovementFrom);
        }

        if (criteria.MovementTo is not null)
        {
            query = query.Where(v => EarliestMovementAtUtc(v) <= criteria.MovementTo);
        }

        if (!string.IsNullOrWhiteSpace(criteria.CreatedBy))
        {
            query = query.Where(v => VisitCreatedBy(v)?.Equals(criteria.CreatedBy, StringComparison.OrdinalIgnoreCase) == true);
        }

        var ordered = query.OrderByDescending(v => v.CreatedAtUtc).ToList();
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToList();

        var result = new VisitSearchResult(items, criteria.Page, criteria.PageSize, totalCount);
        return Task.FromResult(result);
    }

    private static DateTimeOffset LatestMovementAtUtc(Visit visit)
    {
        var timestamps = MovementTimestamps(visit).ToList();
        return timestamps.Count == 0 ? visit.CreatedAtUtc : timestamps.Max();
    }

    private static DateTimeOffset EarliestMovementAtUtc(Visit visit)
    {
        var timestamps = MovementTimestamps(visit).ToList();
        return timestamps.Count == 0 ? visit.CreatedAtUtc : timestamps.Min();
    }

    private static IEnumerable<DateTimeOffset> MovementTimestamps(Visit visit)
    {
        if (visit.AtGateAtUtc is not null)
        {
            yield return visit.AtGateAtUtc.Value;
        }

        if (visit.OnSiteAtUtc is not null)
        {
            yield return visit.OnSiteAtUtc.Value;
        }

        if (visit.CompletedAtUtc is not null)
        {
            yield return visit.CompletedAtUtc.Value;
        }
    }

    private static string? VisitCreatedBy(Visit visit)
    {
        // The initial pre-registration transition's originating actor represents the creator.
        var firstTransition = visit.StatusHistory
            .OrderBy(t => t.ChangedAtUtc)
            .FirstOrDefault();

        return firstTransition?.OriginatingActor;
    }
}
