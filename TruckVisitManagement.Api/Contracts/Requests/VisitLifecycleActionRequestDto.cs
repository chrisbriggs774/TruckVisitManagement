using System.ComponentModel.DataAnnotations;

namespace TruckVisitManagement.Api.Contracts.Requests;

/// <summary>
/// Request body for explicit visit lifecycle actions.
/// </summary>
public sealed class VisitLifecycleActionRequestDto
{
    /// <summary>The actor (operator/system) performing the action.</summary>
    [Required]
    public string OriginatingActor { get; set; } = string.Empty;
}
