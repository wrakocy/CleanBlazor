using Wrak.Clean.Blazor.Core.Shared.Events;
using Wrak.Clean.Blazor.Core.Shared.Features;
using Wrak.Clean.Blazor.Infrastructure;

namespace Wrak.Clean.Blazor.UnitTests.Infrastructure;

public class AppBus_Send
{
    private class MockEvent : ApplicationEventBase { }
    private class MockQuery : QueryBase<MockResponse> { }
    private class MockVoidCommand : CommandBase { }
    private class MockResponseCommand : CommandBase<MockResponse> { }
    private class MockResponse { }

    private readonly Mock<IMediator> _mediator;
    private readonly AppBus _bus;
    private readonly CancellationToken _token;

    public AppBus_Send()
    {
        _mediator = new Mock<IMediator>();
        _bus = new AppBus(_mediator.Object);
        _token = new CancellationToken();
    }

    [Fact]
    public async Task VoidCommand()
    {
        var command = new MockVoidCommand();
        await _bus.Send(command, _token);
        _mediator.Verify(x => x.Send<CommandBase>(command, _token), Times.Once());
    }

    [Fact]
    public async Task ResponseCommand()
    {
        var command = new MockResponseCommand();
        await _bus.Send(command, _token);
        _mediator.Verify(x => x.Send(command, _token), Times.Once());
    }

    [Fact]
    public async Task Query()
    {
        var query = new MockQuery();
        await _bus.Send(query, _token);
        _mediator.Verify(x => x.Send(query, _token), Times.Once());
    }
}
