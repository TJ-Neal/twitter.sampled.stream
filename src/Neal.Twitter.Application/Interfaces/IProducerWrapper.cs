namespace Neal.Twitter.Application.Interfaces;

public interface IProducerWrapper<TMessage> where TMessage : class
{
    abstract Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
}