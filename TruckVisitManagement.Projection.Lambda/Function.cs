using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Microsoft.Extensions.DependencyInjection;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Infrastructure.Persistence.InMemory;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TruckVisitManagement.Projection.Lambda;

public sealed class Function
{
    public async Task HandleAsync(DynamoDBEvent streamEvent, ILambdaContext context)
    {
        var serviceProvider = ConfigureServices();
        var functionHandler = serviceProvider.GetRequiredService<IFunctionHandler>();

        if (functionHandler == null)
        {
            throw new InvalidOperationException("No IFunctionHandler could be found.");
        }

        await functionHandler.HandleAsync(streamEvent, context);
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IVisitWriteStore, InMemoryVisitStore>();
        services.AddSingleton<IVisitReadStore, InMemoryVisitStore>();
        services.AddSingleton<IFunctionHandler, FunctionHandler>();

        return services.BuildServiceProvider();
    }
}