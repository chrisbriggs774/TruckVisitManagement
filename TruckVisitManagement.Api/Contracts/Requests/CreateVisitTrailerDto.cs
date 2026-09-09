using System.ComponentModel.DataAnnotations;

namespace TruckVisitManagement.Api.Contracts.Requests;

public sealed class CreateVisitTrailerDto
{
    /// <summary>Trailer number. Whitespace is stripped and value is capitalized.</summary>
    [Required]
    public string Number { get; set; } = string.Empty;

    /// <summary>Trailer registration. Whitespace is stripped and value is capitalized.</summary>
    [Required]
    public string Registration { get; set; } = string.Empty;
}