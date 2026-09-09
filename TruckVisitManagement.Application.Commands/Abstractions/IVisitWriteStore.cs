using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Commands.Abstractions;

/// <summary>
/// Write-side persistence abstraction for the Visit aggregate.
/// Per ADR-001 this is backed by the transactional command store (DynamoDB in production).
/// </summary>
public interface IVisitWriteStore
{
    Task AddAsync(Visit visit, CancellationToken cancellationToken = default);

    Task<Visit?> GetByIdAsync(VisitId visitId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Visit visit, CancellationToken cancellationToken = default);
}
