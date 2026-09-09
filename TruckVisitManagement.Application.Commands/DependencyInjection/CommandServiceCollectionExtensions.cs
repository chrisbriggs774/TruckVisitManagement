using Microsoft.Extensions.DependencyInjection;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Commands.CreateVisit;
using TruckVisitManagement.Application.Commands.UpdateVisitStatus;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;

namespace TruckVisitManagement.Application.Commands.DependencyInjection;

public static class CommandServiceCollectionExtensions
{
    /// <summary>
    /// Registers all command handlers for the write side.
    /// The <see cref="IVisitWriteStore"/> implementation must be registered separately
    /// by the composition root (e.g. an in-memory store for local runs, DynamoDB in production).
    /// </summary>
    public static IServiceCollection AddVisitCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateVisitCommand, Visit>, CreateVisitCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateVisitStatusCommand, Visit>, UpdateVisitStatusCommandHandler>();

        return services;
    }
}
