using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Requests;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.IntegrationTests.Infrastructure;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.IntegrationTests;

public sealed class VisitLifecycleEndpointsTests : ApiIntegrationTestBase
{
    public VisitLifecycleEndpointsTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ArriveAtGate_TransitionsVisitToAtGate()
    {
        var created = await CreateVisitAsync();

        var visit = await TransitionAsync(created.Id, "arrive-at-gate");

        visit.Status.Should().Be(VisitStatus.AtGate);
    }

    [Fact]
    public async Task EnterSite_TransitionsVisitToOnSite()
    {
        var created = await CreateVisitAsync();
        await TransitionAsync(created.Id, "arrive-at-gate");

        var visit = await TransitionAsync(created.Id, "enter-site");

        visit.Status.Should().Be(VisitStatus.OnSite);
    }

    [Fact]
    public async Task Complete_TransitionsVisitToCompleted()
    {
        var created = await CreateVisitAsync();
        await TransitionAsync(created.Id, "arrive-at-gate");
        await TransitionAsync(created.Id, "enter-site");

        var visit = await TransitionAsync(created.Id, "complete");

        visit.Status.Should().Be(VisitStatus.Completed);
    }

    private async Task<VisitResponseDto> TransitionAsync(Guid id, string action)
    {
        var request = new VisitLifecycleActionRequestDto { OriginatingActor = "integration-test" };

        var response = await Client.PostAsJsonAsync($"/api/visits/{id}/{action}", request, JsonOptions);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var visit = await response.Content.ReadFromJsonAsync<VisitResponseDto>(JsonOptions);
        return visit!;
    }
}
