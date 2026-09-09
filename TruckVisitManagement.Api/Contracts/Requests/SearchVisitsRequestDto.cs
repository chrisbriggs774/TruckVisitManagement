using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.Contracts.Requests;

/// <summary>
/// Query-string parameters for searching visits, as documented in the README.
/// </summary>
public sealed class SearchVisitsRequestDto
{
    /// <summary>Filter by terminal identifier.</summary>
    public string? TerminalId { get; set; }

    /// <summary>Filter by current visit status.</summary>
    public VisitStatus? CurrentStatus { get; set; }

    /// <summary>Filter movements occurring on or after this timestamp.</summary>
    public DateTimeOffset? MovementFrom { get; set; }

    /// <summary>Filter movements occurring on or before this timestamp.</summary>
    public DateTimeOffset? MovementTo { get; set; }

    /// <summary>Filter visits created on or after this timestamp.</summary>
    public DateTimeOffset? CreatedTimeFrom { get; set; }

    /// <summary>Filter visits created on or before this timestamp.</summary>
    public DateTimeOffset? CreatedTimeTo { get; set; }

    /// <summary>Filter by the actor who created the visit.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>1-based page number. Defaults to 1.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Page size. Defaults to 25.</summary>
    public int PageSize { get; set; } = 25;
}
