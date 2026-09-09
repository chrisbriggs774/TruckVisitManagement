using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Tests.TestSupport;

namespace TruckVisitManagement.Api.Tests;

public class GetVisitByIdEndpointBusinessTests
{
    [Fact]
    public async Task GetVisitById_ShouldReturnCreatedVisit()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);

        var response = await client.GetAsync($"/api/visits/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetVisitById_ShouldReturnNotFound_ForUnknownVisit()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync($"/api/visits/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetVisitById_ShouldRequireAuthentication()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync($"/api/visits/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
