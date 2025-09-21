using RabbitMQ.Client;

namespace EventBus.EventBusRabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private IConnection? _connection;

        public DefaultRabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected => _connection is { IsOpen: true };

        public async Task<bool> TryConnectAsync()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect...");

            _connection = await _connectionFactory.CreateConnectionAsync();

            if (IsConnected)
            {
                _logger.LogInformation("RabbitMQ persistent connection acquired a connection.");
                return true;
            }

            _logger.LogError("RabbitMQ connections could not be created and opened.");
            return false;
        }

        public async ValueTask<IChannel> CreateChannelAsync()
        {
            if (!IsConnected)
            {
                await TryConnectAsync();
            }

            if (_connection == null)
            {
                throw new InvalidOperationException("No RabbitMQ connection is available.");
            }

            return await _connection.CreateChannelAsync();
        }

        public void Dispose()
        {
            try
            {
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while disposing RabbitMQ connection");
            }
        }
    }
}
