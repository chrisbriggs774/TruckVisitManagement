using System.ComponentModel.DataAnnotations;

namespace TruckVisitManagement.Api.Contracts.Requests;

public sealed class CreateVisitTruckDto
{
    /// <summary>Truck license plate. Whitespace is stripped and value is capitalized.</summary>
    [Required]
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>Truck unit number. Whitespace is stripped and value is capitalized.</summary>
    [Required]
    public string UnitNumber { get; set; } = string.Empty;
}