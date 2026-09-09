using Microsoft.Extensions.DependencyInjection;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Infrastructure.Persistence.DynamoDb;
using TruckVisitManagement.Infrastructure.Persistence.InMemory;
using TruckVisitManagement.Infrastructure.Persistence.OpenSearch;

namespace TruckVisitManagement.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure persistence implementations.
    /// Supported providers:
    /// - InMemory (default)
    /// - DynamoDbOpenSearch (placeholder registrations for future implementation)
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string provider = "InMemory")
    {
        if (string.Equals(provider, "DynamoDbOpenSearch", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IVisitWriteStore, DynamoDbVisitWriteStore>();
            services.AddSingleton<IVisitReadStore, OpenSearchVisitReadStore>();
            return services;
        }

        services.AddSingleton<InMemoryVisitStore>();
        services.AddSingleton<IVisitWriteStore>(sp => sp.GetRequiredService<InMemoryVisitStore>());
        services.AddSingleton<IVisitReadStore>(sp => sp.GetRequiredService<InMemoryVisitStore>());

        return services;
    }
}
