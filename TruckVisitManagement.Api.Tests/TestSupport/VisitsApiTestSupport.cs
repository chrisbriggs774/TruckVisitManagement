using System.Net.Http.Json;

namespace TruckVisitManagement.Api.Tests.TestSupport;

internal static class VisitsApiTestSupport
{
    public static object BuildCreateVisitRequest(
        string? terminalId = null,
        string? externalDriverId = "DRIVER-123",
        string? licensePlate = "AA11AAA",
        string? unitNumber = "UNIT001",
        string? trailerNumber = "TRAIL001",
        string? trailerRegistration = "REG001")
        => new
        {
            terminalId = terminalId ?? $"TERM-{Guid.NewGuid():N}",
            truck = new
            {
                licensePlate,
                unitNumber
            },
            trailer = new
            {
                number = trailerNumber,
                registration = trailerRegistration
            },
            externalDriverId,
            collections = new[]
            {
                new
                {
                    reference = "COL-1",
                    description = "Collection movement"
                }
            },
            deliveries = new[]
            {
                new
                {
                    reference = "DEL-1",
                    description = "Delivery movement"
                }
            }
        };

    public static async Task<VisitResponse> CreateVisitAsync(HttpClient client, string? terminalId = null)
    {
        var response = await client.PostAsJsonAsync("/api/visits", BuildCreateVisitRequest(terminalId: terminalId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<VisitResponse>())!;
    }

    public static Task<VisitResponse> ArriveAtGateAsync(HttpClient client, Guid visitId, string actor)
        => ExecuteLifecycleActionAsync(client, visitId, "arrive-at-gate", actor);

    public static Task<VisitResponse> EnterSiteAsync(HttpClient client, Guid visitId, string actor)
        => ExecuteLifecycleActionAsync(client, visitId, "enter-site", actor);

    public static Task<VisitResponse> CompleteAsync(HttpClient client, Guid visitId, string actor)
        => ExecuteLifecycleActionAsync(client, visitId, "complete", actor);

    private static async Task<VisitResponse> ExecuteLifecycleActionAsync(HttpClient client, Guid visitId, string action, string actor)
    {
        var response = await client.PostAsJsonAsync($"/api/visits/{visitId}/{action}", new
        {
            originatingActor = actor
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<VisitResponse>())!;
    }
}

internal sealed class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public int TotalPages { get; init; }
}

internal sealed class VisitResponse
{
    public Guid Id { get; init; }
    public string TerminalId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public TruckResponse Truck { get; init; } = new();
    public TrailerResponse? Trailer { get; init; }
    public IReadOnlyList<StatusTransitionResponse> StatusHistory { get; init; } = Array.Empty<StatusTransitionResponse>();
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? AtGateAtUtc { get; init; }
    public DateTimeOffset? OnSiteAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; init; }
}

internal sealed class TruckResponse
{
    public string LicensePlate { get; init; } = string.Empty;
    public string UnitNumber { get; init; } = string.Empty;
}

internal sealed class TrailerResponse
{
    public string Number { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
}

internal sealed class StatusTransitionResponse
{
    public Guid Id { get; init; }
    public string PreviousStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
    public string OriginatingActor { get; init; } = string.Empty;
}