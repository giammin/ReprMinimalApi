using ReprMinimalApi.Core;

namespace ReprMinimalApi.Subscribers;

public class DefaultPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish<T>(T message, CancellationToken cancellationToken)
    {
        var subscribers = _serviceProvider.GetRequiredService<IEnumerable<ISubscriber<T>>>();

        await Task.WhenAll(subscribers.Select(x => x.Handle(message, cancellationToken)));
    }
}