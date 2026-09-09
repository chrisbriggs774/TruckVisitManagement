namespace TruckVisitManagement.Api.Contracts.Responses;

public sealed class TrailerResponseDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
}