namespace TruckVisitManagement.Api.Contracts.Responses;

/// <summary>
/// A page of results together with paging metadata.
/// </summary>
/// <typeparam name="T">The type of item contained in the page.</typeparam>
public sealed class PagedResponseDto<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public int TotalPages { get; init; }
}
