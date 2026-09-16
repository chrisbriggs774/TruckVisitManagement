using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.IntegrationTests.Infrastructure;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Api.IntegrationTests;

public sealed class CreateVisitEndpointTests : ApiIntegrationTestBase
{
    public CreateVisitEndpointTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateVisit_WithValidRequest_ReturnsCreated()
    {
        var request = BuildCreateVisitRequest();

        var response = await Client.PostAsJsonAsync("/api/visits", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var visit = await response.Content.ReadFromJsonAsync<VisitResponseDto>(JsonOptions);
        visit.Should().NotBeNull();
        visit!.Id.Should().NotBeEmpty();
        visit.TerminalId.Should().Be(request.TerminalId);
        visit.Status.Should().Be(VisitStatus.PreRegistered);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateVisit_WithMissingTerminal_ReturnsBadRequest()
    {
        var request = BuildCreateVisitRequest(terminalId: string.Empty);

        var response = await Client.PostAsJsonAsync("/api/visits", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
