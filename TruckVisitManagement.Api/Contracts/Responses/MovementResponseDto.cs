namespace TruckVisitManagement.Api.Contracts.Responses;

public sealed class MovementResponseDto
{
    public Guid Id { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}