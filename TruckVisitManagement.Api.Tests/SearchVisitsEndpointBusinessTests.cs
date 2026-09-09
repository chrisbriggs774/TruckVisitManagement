using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Tests.TestSupport;

namespace TruckVisitManagement.Api.Tests;

public class SearchVisitsEndpointBusinessTests
{
    [Fact]
    public async Task SearchVisits_ShouldAcceptAllDocumentedQueryParameters()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var terminalId = $"TERM-{Guid.NewGuid():N}";
        var created = await VisitsApiTestSupport.CreateVisitAsync(client, terminalId);

        var atGate = await VisitsApiTestSupport.ArriveAtGateAsync(client, created.Id, "driver-search");
        var movementReference = atGate.AtGateAtUtc ?? DateTimeOffset.UtcNow;

        var query = string.Join("&", new[]
        {
            $"terminalId={Uri.EscapeDataString(terminalId)}",
            "currentStatus=AtGate",
            $"movementFrom={Uri.EscapeDataString(movementReference.AddMinutes(-5).ToString("O"))}",
            $"movementTo={Uri.EscapeDataString(movementReference.AddMinutes(5).ToString("O"))}",
            $"createdTimeFrom={Uri.EscapeDataString(created.CreatedAtUtc.AddMinutes(-5).ToString("O"))}",
            $"createdTimeTo={Uri.EscapeDataString(created.CreatedAtUtc.AddMinutes(5).ToString("O"))}",
            "createdBy=driver-search",
            "page=1",
            "pageSize=10"
        });

        var response = await client.GetAsync($"/api/visits?{query}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchVisits_ShouldReturnBadRequest_WhenCurrentStatusIsInvalid()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/visits?currentStatus=UnknownStatus");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchVisits_ShouldReturnBadRequest_WhenPagingIsInvalid()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/visits?page=-1&pageSize=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchVisits_ShouldRequireAuthentication()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/visits?page=1&pageSize=25");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SearchVisits_ShouldRejectUnauthorizedTerminalAccess()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/visits?terminalId=UNAUTHORIZED-TERMINAL");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
