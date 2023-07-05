using System.Security.Cryptography;
using ReprMinimalApi.Core;

namespace ReprMinimalApi.Subscribers;

public record PongMessage(string Message);
public record PingMessage(string Message);


public class Pong1Subscriber : ISubscriber<PongMessage>
{
    private readonly ILogger<Pong1Subscriber> _logger;

    public Pong1Subscriber(ILogger<Pong1Subscriber> logger)
    {
        _logger = logger;
    }
    public async Task Handle(PongMessage message, CancellationToken cancellationToken)
    {
        await Task.Delay(RandomNumberGenerator.GetInt32(1, 300), cancellationToken);
        _logger.LogInformation("{type} handle {message}", GetType(), message);
    }
}
public class Pong2Subscriber : ISubscriber<PongMessage>
{
    private readonly ILogger<Pong2Subscriber> _logger;

    public Pong2Subscriber(ILogger<Pong2Subscriber> logger)
    {
        _logger = logger;
    }
    public async Task Handle(PongMessage message, CancellationToken cancellationToken)
    {
        await Task.Delay(RandomNumberGenerator.GetInt32(1, 300), cancellationToken);
        _logger.LogInformation("{type} handle {message}", GetType(), message);
    }
}

public class PIng2Subscriber : ISubscriber<PingMessage>
{
    private readonly ILogger<PIng2Subscriber> _logger;

    public PIng2Subscriber(ILogger<PIng2Subscriber> logger)
    {
        _logger = logger;
    }
    public async Task Handle(PingMessage message, CancellationToken cancellationToken)
    {
        await Task.Delay(RandomNumberGenerator.GetInt32(1, 300), cancellationToken);
        _logger.LogInformation("{type} handle {message}", GetType(), message);
    }
}
public class Ping2Subscriber : ISubscriber<PingMessage>
{
    private readonly ILogger<Ping2Subscriber> _logger;

    public Ping2Subscriber(ILogger<Ping2Subscriber> logger)
    {
        _logger = logger;
    }
    public async Task Handle(PingMessage message, CancellationToken cancellationToken)
    {
        await Task.Delay(RandomNumberGenerator.GetInt32(1, 300), cancellationToken);
        _logger.LogInformation("{type} handle {message}", GetType(), message);
    }
}