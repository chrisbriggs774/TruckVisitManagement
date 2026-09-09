namespace TruckVisitManagement.Application.Queries.Abstractions;

/// <summary>
/// Marker for a query that produces a result of <typeparamref name="TResult"/>.
/// </summary>
public interface IQuery<TResult>
{
}

/// <summary>
/// Handles a query of type <typeparamref name="TQuery"/> and returns <typeparamref name="TResult"/>.
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
