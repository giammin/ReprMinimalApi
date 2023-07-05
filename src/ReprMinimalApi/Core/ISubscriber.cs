namespace ReprMinimalApi.Core;

public interface ISubscriber<T>
{
    Task Handle(T message, CancellationToken cancellationToken);
}