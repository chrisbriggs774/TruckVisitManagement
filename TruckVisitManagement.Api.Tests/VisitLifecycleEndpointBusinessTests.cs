using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Tests.TestSupport;

namespace TruckVisitManagement.Api.Tests;

public class VisitLifecycleEndpointBusinessTests
{
    [Fact]
    public async Task LifecycleEndpoints_ShouldSupportSequentialLifecycleTransitions()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);

        await VisitsApiTestSupport.ArriveAtGateAsync(client, created.Id, "driver-1");
        await VisitsApiTestSupport.EnterSiteAsync(client, created.Id, "driver-1");
        var completed = await VisitsApiTestSupport.CompleteAsync(client, created.Id, "driver-1");

        Assert.Equal("Completed", completed.Status);
        Assert.NotNull(completed.AtGateAtUtc);
        Assert.NotNull(completed.OnSiteAtUtc);
        Assert.NotNull(completed.CompletedAtUtc);
    }

    [Fact]
    public async Task ArriveAtGate_ShouldReturnNotFound_ForUnknownVisit()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/visits/{Guid.NewGuid()}/arrive-at-gate", new
        {
            originatingActor = "driver-2"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Complete_ShouldRejectNonSequentialTransition()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);

        var response = await client.PostAsJsonAsync($"/api/visits/{created.Id}/complete", new
        {
            originatingActor = "driver-3"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task EnterSite_ShouldRejectStatusRegression()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);
        await VisitsApiTestSupport.ArriveAtGateAsync(client, created.Id, "driver-4");

        var response = await client.PostAsJsonAsync($"/api/visits/{created.Id}/arrive-at-gate", new
        {
            originatingActor = "driver-4"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task EnterSite_ShouldRejectTransition_AfterCompletion()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);

        await VisitsApiTestSupport.ArriveAtGateAsync(client, created.Id, "driver-5");
        await VisitsApiTestSupport.EnterSiteAsync(client, created.Id, "driver-5");
        await VisitsApiTestSupport.CompleteAsync(client, created.Id, "driver-5");

        var response = await client.PostAsJsonAsync($"/api/visits/{created.Id}/enter-site", new
        {
            originatingActor = "driver-5"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ArriveAtGate_ShouldReturnBadRequest_WhenActorIsMissing()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);

        var response = await client.PostAsJsonAsync($"/api/visits/{created.Id}/arrive-at-gate", new
        {
            originatingActor = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LifecycleEndpoints_ShouldKeepAuditHistoryImmutable_WhenTransitionFails()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var created = await VisitsApiTestSupport.CreateVisitAsync(client);
        var atGate = await VisitsApiTestSupport.ArriveAtGateAsync(client, created.Id, "driver-7");

        var failed = await client.PostAsJsonAsync($"/api/visits/{created.Id}/arrive-at-gate", new
        {
            originatingActor = "driver-7"
        });

        Assert.Equal(HttpStatusCode.Conflict, failed.StatusCode);

        var readAfterFailure = await client.GetAsync($"/api/visits/{created.Id}");
        var current = await readAfterFailure.Content.ReadFromJsonAsync<VisitResponse>();

        Assert.NotNull(current);
        Assert.Equal(atGate.StatusHistory.Count, current!.StatusHistory.Count);
        Assert.Equal(atGate.StatusHistory[0].Id, current.StatusHistory[0].Id);
    }

    [Fact]
    public async Task ArriveAtGate_ShouldRequireAuthentication()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/visits/{Guid.NewGuid()}/arrive-at-gate", new
        {
            originatingActor = "driver-8"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
