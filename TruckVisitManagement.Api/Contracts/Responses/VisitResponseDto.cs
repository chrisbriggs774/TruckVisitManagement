using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.Contracts.Responses;

/// <summary>
/// Full representation of a visit returned by the API.
/// </summary>
public sealed class VisitResponseDto
{
    public Guid Id { get; init; }
    public string TerminalId { get; init; } = string.Empty;
    public VisitStatus Status { get; init; }
    public TruckResponseDto Truck { get; init; } = new();
    public TrailerResponseDto? Trailer { get; init; }
    public DriverReferenceResponseDto Driver { get; init; } = new();
    public IReadOnlyCollection<MovementResponseDto> Collections { get; init; } = Array.Empty<MovementResponseDto>();
    public IReadOnlyCollection<MovementResponseDto> Deliveries { get; init; } = Array.Empty<MovementResponseDto>();
    public IReadOnlyCollection<VisitStatusTransitionResponseDto> StatusHistory { get; init; } = Array.Empty<VisitStatusTransitionResponseDto>();
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? AtGateAtUtc { get; init; }
    public DateTimeOffset? OnSiteAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
}