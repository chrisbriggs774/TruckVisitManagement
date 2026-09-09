using TruckVisitManagement.Application.Queries.Abstractions;

namespace TruckVisitManagement.Application.Queries.SearchVisits;

public sealed class SearchVisitsQueryHandler : IQueryHandler<SearchVisitsQuery, VisitSearchResult>
{
    private const int MaxPageSize = 200;

    private readonly IVisitReadStore _readStore;

    public SearchVisitsQueryHandler(IVisitReadStore readStore)
    {
        _readStore = readStore ?? throw new ArgumentNullException(nameof(readStore));
    }

    public Task<VisitSearchResult> HandleAsync(SearchVisitsQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize switch
        {
            < 1 => 25,
            > MaxPageSize => MaxPageSize,
            _ => query.PageSize
        };

        var criteria = new VisitSearchCriteria(
            query.TerminalId,
            query.CurrentStatus,
            query.MovementFrom,
            query.MovementTo,
            query.CreatedTimeFrom,
            query.CreatedTimeTo,
            query.CreatedBy,
            page,
            pageSize);

        return _readStore.SearchAsync(criteria, cancellationToken);
    }
}
