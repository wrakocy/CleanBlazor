using Wrak.Clean.Blazor.Core.Shared.Events;
using Wrak.Clean.Blazor.Infrastructure;

namespace Wrak.Clean.Blazor.UnitTests.Infrastructure;

public class AppBus_Publish
{
    private class MockEvent : ApplicationEventBase { }
    private class MockResponse { }

    private readonly Mock<IMediator> _mediator;
    private readonly AppBus _bus;
    private readonly CancellationToken _token;

    public AppBus_Publish()
    {
        _mediator = new Mock<IMediator>();
        _bus = new AppBus(_mediator.Object);
        _token = new CancellationToken();
    }

    [Fact]
    public async Task ApplicationEvent()
    {
        var e = new MockEvent();
        await _bus.Publish(e, _token);
        _mediator.Verify(x => x.Publish<ApplicationEventBase>(e, _token), Times.Once());
    }
}
