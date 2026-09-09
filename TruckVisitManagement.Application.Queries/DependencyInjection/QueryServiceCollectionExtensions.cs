using Microsoft.Extensions.DependencyInjection;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Application.Queries.GetVisitById;
using TruckVisitManagement.Application.Queries.SearchVisits;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;

namespace TruckVisitManagement.Application.Queries.DependencyInjection;

public static class QueryServiceCollectionExtensions
{
    /// <summary>
    /// Registers all query handlers for the read side.
    /// The <see cref="IVisitReadStore"/> implementation must be registered separately
    /// by the composition root (e.g. an in-memory store for local runs, OpenSearch in production).
    /// </summary>
    public static IServiceCollection AddVisitQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetVisitByIdQuery, Visit?>, GetVisitByIdQueryHandler>();
        services.AddScoped<IQueryHandler<SearchVisitsQuery, VisitSearchResult>, SearchVisitsQueryHandler>();

        return services;
    }
}
