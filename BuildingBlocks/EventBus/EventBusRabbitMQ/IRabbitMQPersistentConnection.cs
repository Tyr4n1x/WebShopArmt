using RabbitMQ.Client;

namespace EventBus.EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        Task<bool> TryConnectAsync();
        ValueTask<IChannel> CreateChannelAsync();
    }
}
