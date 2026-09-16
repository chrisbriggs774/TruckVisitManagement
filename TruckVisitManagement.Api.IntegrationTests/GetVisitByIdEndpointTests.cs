using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.IntegrationTests.Infrastructure;

namespace TruckVisitManagement.Api.IntegrationTests;

public sealed class GetVisitByIdEndpointTests : ApiIntegrationTestBase
{
    public GetVisitByIdEndpointTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetVisitById_WhenVisitExists_ReturnsVisit()
    {
        var created = await CreateVisitAsync();

        var response = await Client.GetAsync($"/api/visits/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var visit = await response.Content.ReadFromJsonAsync<VisitResponseDto>(JsonOptions);
        visit.Should().NotBeNull();
        visit!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetVisitById_WhenVisitDoesNotExist_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/visits/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
