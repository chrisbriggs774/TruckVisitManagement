using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Tests.TestSupport;

namespace TruckVisitManagement.Api.Tests;

public class CreateVisitEndpointBusinessTests
{
    [Fact]
    public async Task CreateVisit_ShouldReturnCreated_AndNormalizeVehicleIdentifiers()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/visits", VisitsApiTestSupport.BuildCreateVisitRequest(
            terminalId: "term-001",
            licensePlate: "yx 22 abc",
            unitNumber: "ab 123",
            trailerNumber: "tr 01",
            trailerRegistration: "xy 77 zz"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<VisitResponse>();
        Assert.NotNull(created);
        Assert.Equal("YX22ABC", created!.Truck.LicensePlate);
        Assert.Equal("AB123", created.Truck.UnitNumber);
        Assert.Equal("TR01", created.Trailer!.Number);
        Assert.Equal("XY77ZZ", created.Trailer.Registration);
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnBadRequest_WhenTerminalIdIsMissing()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/visits", VisitsApiTestSupport.BuildCreateVisitRequest(terminalId: ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnBadRequest_WhenDriverReferenceIsMissing()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/visits", VisitsApiTestSupport.BuildCreateVisitRequest(externalDriverId: ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVisit_ShouldReturnBadRequest_WhenTruckIdentifiersAreWhitespaceOnly()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/visits", VisitsApiTestSupport.BuildCreateVisitRequest(
            licensePlate: "   ",
            unitNumber: "  "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVisit_ShouldRequireAuthentication()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/visits", VisitsApiTestSupport.BuildCreateVisitRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
