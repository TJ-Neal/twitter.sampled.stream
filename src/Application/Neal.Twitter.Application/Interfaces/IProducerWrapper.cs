namespace Neal.Twitter.Application.Interfaces;

/// <summary>
/// Represents the wrapper functionality for an event producer using a <typeparamref name="TMessage"/> message to produce an event.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IProducerWrapper<TMessage> where TMessage : class
{
    abstract Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
}