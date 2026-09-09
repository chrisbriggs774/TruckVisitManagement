using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Infrastructure.Persistence.OpenSearch;

/// <summary>
/// Placeholder read-side store for OpenSearch.
/// </summary>
public sealed class OpenSearchVisitReadStore : IVisitReadStore
{
    public Task<Visit?> GetByIdAsync(VisitId visitId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("OpenSearch read store is not implemented yet.");

    public Task<VisitSearchResult> SearchAsync(VisitSearchCriteria criteria, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("OpenSearch read store is not implemented yet.");
}
