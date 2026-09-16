using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TruckVisitManagement.Api.Contracts.Responses;
using TruckVisitManagement.Api.IntegrationTests.Infrastructure;

namespace TruckVisitManagement.Api.IntegrationTests;

public sealed class SearchVisitsEndpointTests : ApiIntegrationTestBase
{
    public SearchVisitsEndpointTests(WebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SearchVisits_ReturnsPagedResults()
    {
        var terminalId = $"TERMINAL-{Guid.NewGuid():N}".ToUpperInvariant();
        await CreateVisitAsync(BuildCreateVisitRequest(terminalId: terminalId));

        var response = await Client.GetAsync($"/api/visits?terminalId={terminalId}&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var page = await response.Content.ReadFromJsonAsync<PagedResponseDto<VisitResponseDto>>(JsonOptions);
        page.Should().NotBeNull();
        page!.Page.Should().Be(1);
        page.PageSize.Should().Be(10);
        page.Items.Should().OnlyContain(v => v.TerminalId == terminalId);
        page.TotalCount.Should().BeGreaterThan(0);
    }
}
