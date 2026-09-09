using System.ComponentModel.DataAnnotations;

namespace TruckVisitManagement.Api.Contracts.Requests;

public sealed class CreateVisitMovementDto
{
    /// <summary>Movement reference. Whitespace is trimmed and value is capitalized.</summary>
    [Required]
    public string Reference { get; set; } = string.Empty;

    /// <summary>Free-text description of the movement.</summary>
    public string Description { get; set; } = string.Empty;
}