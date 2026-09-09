namespace TruckVisitManagement.Api.Contracts.Responses;

public sealed class DriverReferenceResponseDto
{
    public Guid Id { get; init; }
    public string ExternalDriverId { get; init; } = string.Empty;
}