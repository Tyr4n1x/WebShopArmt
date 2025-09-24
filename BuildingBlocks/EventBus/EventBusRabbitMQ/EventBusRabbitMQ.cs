using EventBus.Events;
using EventBus.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventBus.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly ILogger<EventBusRabbitMQ> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, List<Type>> _handlers = [];
        private readonly List<Type> _eventTypes = [];

        public EventBusRabbitMQ(IRabbitMQPersistentConnection connection, IServiceProvider serviceProvider, ILogger<EventBusRabbitMQ> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            if (!_connection.IsConnected)
            {
               await _connection.TryConnectAsync();
            }

            using var channel = await _connection.CreateChannelAsync();
            var eventName = @event.GetType().Name;

            await channel.ExchangeDeclareAsync(
                exchange: "event_bus",
                type: ExchangeType.Direct);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "event_bus",
                routingKey: eventName,
                mandatory: true,
                body: body);

            _logger.LogInformation("Published event {EventName}", eventName);
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventType = typeof(T);

            if (!_eventTypes.Contains(eventType))
            {
                _eventTypes.Add(eventType);
            }

            var handlerType = typeof(TH);

            if (!_handlers.TryGetValue(eventType.Name, out List<Type>? value))
            {
                _handlers.Add(eventType.Name, []);
            }

            if (_handlers[eventType.Name].Contains(handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventType.Name}'",
                    nameof(TH));
            }

            _handlers[eventType.Name].Add(handlerType);

            _ = StartBasicConsumeAsync<T>();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventType = typeof(T);
            var handlerType = typeof(TH);

            if (!_handlers[eventType.Name].Contains(handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} not registered for '{eventType.Name}'",
                    nameof(TH));
            }

            _handlers[eventType.Name].Remove(handlerType);

        }

        private async Task StartBasicConsumeAsync<T>() where T : IntegrationEvent
        {
            if (!_connection.IsConnected)
            {
                await _connection.TryConnectAsync();
            }

            var channel = await _connection.CreateChannelAsync();
            var eventName = typeof(T).Name;

            await channel.ExchangeDeclareAsync(
                exchange: "event_bus",
                type: ExchangeType.Direct);

            await channel.QueueDeclareAsync(
                queue: eventName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                queue: eventName,
                exchange: "event_bus",
                routingKey: eventName);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await ProcessEvent(eventName, message);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    _logger.LogError(ex, "Error processing event {EventName}", eventName);
                }
                
            };

            await channel.BasicConsumeAsync(
                queue: eventName,
                autoAck: false,
                consumer: consumer);

        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.TryGetValue(eventName, out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetService(handlerType);

                    if (handler == null) continue;

                    var eventType = _eventTypes.Single(t => t.Name == eventName);
                    var settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType, settings) as IntegrationEvent;

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    var handleMethod = concreteType.GetMethod("HandleAsync");
                    if (handleMethod != null && integrationEvent != null)
                    {
                        await (Task)handleMethod.Invoke(handler, [integrationEvent])!;
                    }
                }
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
