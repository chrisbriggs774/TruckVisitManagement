using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Requests;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.IntegrationTests.Infrastructure;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.IntegrationTests.Scenarios;

/// <summary>
/// Business scenario: a truck visit progresses through its full lifecycle
/// from pre-registration to completion, and each stage is observable via the API.
/// </summary>
public sealed class VisitLifecycleScenarioTests : ApiIntegrationTestBase
{
    public VisitLifecycleScenarioTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task PreRegisteredVisit_ProgressesThroughGateAndSite_ToCompletion()
    {
        // A haulier pre-registers a visit for a collection and a delivery.
        var created = await CreateVisitAsync(BuildCreateVisitRequest(terminalId: "TERMINAL-LIFECYCLE"));
        created.Status.Should().Be(VisitStatus.PreRegistered);

        // The truck arrives at the gate.
        var atGate = await TransitionAsync(created.Id, "arrive-at-gate");
        atGate.Status.Should().Be(VisitStatus.AtGate);
        atGate.AtGateAtUtc.Should().NotBeNull();

        // The truck is admitted onto the site.
        var onSite = await TransitionAsync(created.Id, "enter-site");
        onSite.Status.Should().Be(VisitStatus.OnSite);
        onSite.OnSiteAtUtc.Should().NotBeNull();

        // Work is finished and the visit is completed.
        var completed = await TransitionAsync(created.Id, "complete");
        completed.Status.Should().Be(VisitStatus.Completed);
        completed.CompletedAtUtc.Should().NotBeNull();

        // The full status history is retained and observable via the API.
        var fetched = await Client.GetFromJsonAsync<VisitResponseDto>($"/api/visits/{created.Id}", JsonOptions);
        fetched.Should().NotBeNull();
        fetched!.Status.Should().Be(VisitStatus.Completed);
        fetched.StatusHistory.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Visit_CannotSkipGate_WhenEnteringSiteDirectly()
    {
        // A pre-registered visit cannot enter the site without first arriving at the gate.
        var created = await CreateVisitAsync();

        var response = await PostTransitionAsync(created.Id, "enter-site");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<VisitResponseDto> TransitionAsync(Guid id, string action)
    {
        var response = await PostTransitionAsync(id, action);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var visit = await response.Content.ReadFromJsonAsync<VisitResponseDto>(JsonOptions);
        return visit!;
    }

    private Task<HttpResponseMessage> PostTransitionAsync(Guid id, string action)
    {
        var request = new VisitLifecycleActionRequestDto { OriginatingActor = "scenario-test" };
        return Client.PostAsJsonAsync($"/api/visits/{id}/{action}", request, JsonOptions);
    }
}
