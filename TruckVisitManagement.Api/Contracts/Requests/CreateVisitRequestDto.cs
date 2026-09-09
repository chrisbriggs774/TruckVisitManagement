using System.ComponentModel.DataAnnotations;

namespace TruckVisitManagement.Api.Contracts.Requests;

/// <summary>
/// Request body for creating (pre-registering) a truck visit.
/// </summary>
public sealed class CreateVisitRequestDto
{
    /// <summary>Terminal the visit belongs to.</summary>
    [Required]
    public string TerminalId { get; set; } = string.Empty;

    /// <summary>Truck details. Unit number and license plate are required.</summary>
    [Required]
    public CreateVisitTruckDto Truck { get; set; } = new();

    /// <summary>Optional trailer details.</summary>
    public CreateVisitTrailerDto? Trailer { get; set; }

    /// <summary>External driver identifier captured for the visit.</summary>
    [Required]
    public string ExternalDriverId { get; set; } = string.Empty;

    /// <summary>Collection movements associated with the visit.</summary>
    public IReadOnlyCollection<CreateVisitMovementDto> Collections { get; set; } = Array.Empty<CreateVisitMovementDto>();

    /// <summary>Delivery movements associated with the visit.</summary>
    public IReadOnlyCollection<CreateVisitMovementDto> Deliveries { get; set; } = Array.Empty<CreateVisitMovementDto>();
}