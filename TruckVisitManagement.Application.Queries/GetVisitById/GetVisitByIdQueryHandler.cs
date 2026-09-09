using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Application.Queries.GetVisitById;

public sealed class GetVisitByIdQueryHandler : IQueryHandler<GetVisitByIdQuery, Visit?>
{
    private readonly IVisitReadStore _readStore;

    public GetVisitByIdQueryHandler(IVisitReadStore readStore)
    {
        _readStore = readStore ?? throw new ArgumentNullException(nameof(readStore));
    }

    public Task<Visit?> HandleAsync(GetVisitByIdQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return _readStore.GetByIdAsync(VisitId.From(query.VisitId), cancellationToken);
    }
}
