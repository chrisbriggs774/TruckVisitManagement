using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Domain.VisitManagement.Aggregates;
using TruckVisitManagement.Domain.VisitManagement.Entities;

namespace TruckVisitManagement.Api.Mapping;

/// <summary>
/// Maps domain aggregates returned by command/query handlers to API response DTOs.
/// </summary>
public static class VisitMappingExtensions
{
    public static VisitResponseDto ToResponseDto(this Visit visit)
    {
        ArgumentNullException.ThrowIfNull(visit);

        return new VisitResponseDto
        {
            Id = visit.Id.Value,
            TerminalId = visit.TerminalId.Value,
            Status = visit.Status,
            Truck = visit.Truck.ToResponseDto(),
            Trailer = visit.Trailer?.ToResponseDto(),
            Driver = visit.DriverReference.ToResponseDto(),
            Collections = visit.Collections.Select(c => c.ToResponseDto()).ToList(),
            Deliveries = visit.Deliveries.Select(d => d.ToResponseDto()).ToList(),
            StatusHistory = visit.StatusHistory.Select(t => t.ToResponseDto()).ToList(),
            CreatedAtUtc = visit.CreatedAtUtc,
            AtGateAtUtc = visit.AtGateAtUtc,
            OnSiteAtUtc = visit.OnSiteAtUtc,
            CompletedAtUtc = visit.CompletedAtUtc
        };
    }

    private static TruckResponseDto ToResponseDto(this Truck truck) => new()
    {
        Id = truck.Id,
        LicensePlate = truck.LicensePlate.Value,
        UnitNumber = truck.UnitNumber.Value
    };

    private static TrailerResponseDto ToResponseDto(this Trailer trailer) => new()
    {
        Id = trailer.Id,
        Number = trailer.Number.Value,
        Registration = trailer.Registration.Value
    };

    private static DriverReferenceResponseDto ToResponseDto(this DriverReference driver) => new()
    {
        Id = driver.Id,
        ExternalDriverId = driver.ExternalDriverId
    };

    private static MovementResponseDto ToResponseDto(this Collection collection) => new()
    {
        Id = collection.Id,
        Reference = collection.Reference,
        Description = collection.Description
    };

    private static MovementResponseDto ToResponseDto(this Delivery delivery) => new()
    {
        Id = delivery.Id,
        Reference = delivery.Reference,
        Description = delivery.Description
    };

    private static VisitStatusTransitionResponseDto ToResponseDto(this VisitStatusTransition transition) => new()
    {
        Id = transition.Id,
        PreviousStatus = transition.PreviousStatus,
        NewStatus = transition.NewStatus,
        OriginatingActor = transition.OriginatingActor,
        ChangedAtUtc = transition.ChangedAtUtc
    };
}
