using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Infrastructure.Persistence.DynamoDb;

/// <summary>
/// Placeholder write-side store for DynamoDB.
/// </summary>
public sealed class DynamoDbVisitWriteStore : IVisitWriteStore
{
    public Task AddAsync(Visit visit, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("DynamoDB write store is not implemented yet.");

    public Task<Visit?> GetByIdAsync(VisitId visitId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("DynamoDB write store is not implemented yet.");

    public Task UpdateAsync(Visit visit, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("DynamoDB write store is not implemented yet.");
}
