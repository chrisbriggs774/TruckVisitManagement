using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.Contracts.Responses;

public sealed class VisitStatusTransitionResponseDto
{
    public Guid Id { get; init; }
    public VisitStatus PreviousStatus { get; init; }
    public VisitStatus NewStatus { get; init; }
    public string OriginatingActor { get; init; } = string.Empty;
    public DateTimeOffset ChangedAtUtc { get; init; }
}