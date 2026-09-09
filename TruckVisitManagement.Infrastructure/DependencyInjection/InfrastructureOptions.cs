namespace TruckVisitManagement.Infrastructure.DependencyInjection;

/// <summary>
/// Infrastructure persistence options.
/// </summary>
public sealed class InfrastructureOptions
{
    /// <summary>
    /// Allowed values: InMemory, DynamoDbOpenSearch.
    /// </summary>
    public string Provider { get; set; } = "InMemory";
}
