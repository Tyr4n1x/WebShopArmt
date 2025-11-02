using EventBus;
using EventBus.EventBusRabbitMQ;
using EventBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace UnitTests.EventBus
{
    public class DependencyInjectionTests
    {
        private readonly ITestOutputHelper _testOutput;

        public DependencyInjectionTests(ITestOutputHelper output)
        {
            _testOutput = output;
        }

        [Fact]
        public void AddEventBus_ShouldRegisterAllDependencies()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddEventBus("localhost");

            // Act
            var provider = services.BuildServiceProvider();

            var connection = provider.GetService<IRabbitMQPersistentConnection>();
            var eventBus = provider.GetService<IEventBus>();

            _testOutput.WriteLine($"connection: {connection}");
            _testOutput.WriteLine($"eventBus: {eventBus}");

            // Assert
            Assert.NotNull(connection);
            Assert.NotNull(eventBus);
        }
    }
}
