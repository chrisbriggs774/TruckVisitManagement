using System.ComponentModel.DataAnnotations;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.Contracts.Requests;

/// <summary>
/// Request body for transitioning a visit to a new status.
/// All transitions are retained as an immutable audit history.
/// </summary>
public sealed class UpdateVisitStatusRequestDto
{
    /// <summary>The status to transition the visit to.</summary>
    [Required]
    public VisitStatus TargetStatus { get; set; }

    /// <summary>The actor (operator/system) performing the transition.</summary>
    [Required]
    public string OriginatingActor { get; set; } = string.Empty;
}
