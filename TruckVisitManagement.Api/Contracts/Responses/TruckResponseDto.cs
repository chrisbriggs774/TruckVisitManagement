namespace TruckVisitManagement.Api.Contracts.Responses;

public sealed class TruckResponseDto
{
    public Guid Id { get; init; }
    public string LicensePlate { get; init; } = string.Empty;
    public string UnitNumber { get; init; } = string.Empty;
}