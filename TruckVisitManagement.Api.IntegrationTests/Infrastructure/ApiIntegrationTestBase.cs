using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Requests;
using TruckVisitManagement.Api.Contracts.Responses;

namespace TruckVisitManagement.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for API endpoint integration tests. Spins up the API in-memory
/// via <see cref="WebApplicationFactory{TEntryPoint}"/> and exposes helpers
/// shared across the per-endpoint test files.
/// </summary>
public abstract class ApiIntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected ApiIntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected WebApplicationFactory<Program> Factory { get; }

    protected HttpClient Client { get; }

    /// <summary>Builds a valid create-visit request payload with sensible defaults.</summary>
    protected static CreateVisitRequestDto BuildCreateVisitRequest(
        string terminalId = "TERMINAL-1",
        string licensePlate = "AB12CDE",
        string unitNumber = "UNIT-001",
        string externalDriverId = "DRIVER-1")
        => new()
        {
            TerminalId = terminalId,
            Truck = new CreateVisitTruckDto
            {
                LicensePlate = licensePlate,
                UnitNumber = unitNumber
            },
            ExternalDriverId = externalDriverId,
            Collections = new[]
            {
                new CreateVisitMovementDto { Reference = "COL-1", Description = "Collection 1" }
            },
            Deliveries = new[]
            {
                new CreateVisitMovementDto { Reference = "DEL-1", Description = "Delivery 1" }
            }
        };

    /// <summary>Creates a visit via the API and returns the created representation.</summary>
    protected async Task<VisitResponseDto> CreateVisitAsync(CreateVisitRequestDto? request = null)
    {
        request ??= BuildCreateVisitRequest();

        var response = await Client.PostAsJsonAsync("/api/visits", request, JsonOptions);
        response.EnsureSuccessStatusCode();

        var visit = await response.Content.ReadFromJsonAsync<VisitResponseDto>(JsonOptions);
        return visit!;
    }
}
