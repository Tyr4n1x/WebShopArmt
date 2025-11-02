using EventBus.EventBusRabbitMQ;
using EventBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, string hostName = "localhost")
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory
                {
                    HostName = hostName
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            services.AddSingleton<IEventBus, EventBus.EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBus.EventBusRabbitMQ.EventBusRabbitMQ>>();
                var provider = sp;

                return new EventBus.EventBusRabbitMQ.EventBusRabbitMQ(connection, provider, logger);
            });

            return services;
        }
    }
}
