using FluentAssertions;
using NSubstitute;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Application.Queries.SearchVisits;
using TruckVisitManagement.Domain.VisitManagement.Enums;

namespace TruckVisitManagement.Application.Queries.Tests.SearchVisits;

public class SearchVisitsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldNormalizePageAndPageSizeBeforeDelegating()
    {
        VisitSearchCriteria? criteria = null;
        var store = Substitute.For<IVisitReadStore>();
        store.SearchAsync(Arg.Do<VisitSearchCriteria>(c => criteria = c), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new VisitSearchResult([], 1, 25, 0)));
        var handler = new SearchVisitsQueryHandler(store);

        await handler.HandleAsync(new SearchVisitsQuery(null, null, null, null, null, null, null, 0, 999));

        criteria.Should().NotBeNull();
        criteria!.Page.Should().Be(1);
        criteria.PageSize.Should().Be(200);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassThroughFilterCriteria()
    {
        VisitSearchCriteria? criteria = null;
        var store = Substitute.For<IVisitReadStore>();
        store.SearchAsync(Arg.Do<VisitSearchCriteria>(c => criteria = c), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new VisitSearchResult([], 3, 10, 2)));
        var handler = new SearchVisitsQueryHandler(store);
        var from = DateTimeOffset.UtcNow.AddHours(-1);
        var to = DateTimeOffset.UtcNow;

        await handler.HandleAsync(new SearchVisitsQuery("TERM-1", VisitStatus.AtGate, from, to, from, to, "driver-1", 3, 10));

        criteria.Should().NotBeNull();
        criteria!.TerminalId.Should().Be("TERM-1");
        criteria.CurrentStatus.Should().Be(VisitStatus.AtGate);
        criteria.MovementFrom.Should().Be(from);
        criteria.MovementTo.Should().Be(to);
        criteria.CreatedTimeFrom.Should().Be(from);
        criteria.CreatedTimeTo.Should().Be(to);
        criteria.CreatedBy.Should().Be("driver-1");
        criteria.Page.Should().Be(3);
        criteria.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenQueryIsNull()
    {
        var store = Substitute.For<IVisitReadStore>();
        var handler = new SearchVisitsQueryHandler(store);

        var act = async () => await handler.HandleAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
