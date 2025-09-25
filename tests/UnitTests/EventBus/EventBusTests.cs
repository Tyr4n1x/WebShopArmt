using EventBus.EventBusRabbitMQ;
using EventBus.Events;
using EventBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.EventBus
{
    // A simple fake event
    public class FakeEvent : IntegrationEvent
    {
        public string Value { get; set; } = string.Empty;
    }

    // A handler for the fake event
    public class FakeEventHandler : IIntegrationEventHandler<FakeEvent>
    {
        public bool WasHandled { get; private set; }
        public string? HandledValue { get; private set; }

        public Task HandleAsync(FakeEvent @event)
        {
            WasHandled = true;
            HandledValue = @event.Value;
            return Task.CompletedTask;
        }
    }

    public class EventBusTests
    {
        private readonly ServiceProvider _provider;
        private readonly EventBusRabbitMQ _eventBus;
        private readonly FakeEventHandler _handler;
        private readonly ITestOutputHelper _testOutput;

        public EventBusTests(ITestOutputHelper output)
        {
            _handler = new FakeEventHandler();

            // Create DI container
            var services = new ServiceCollection();
            services.AddSingleton<IIntegrationEventHandler<FakeEvent>>(_handler);

            _provider = services.BuildServiceProvider();

            // Mock connection and logger
            var connection = new Mock<IRabbitMQPersistentConnection>();
            connection.Setup(c => c.IsConnected).Returns(true);

            var logger = new Mock<ILogger<EventBusRabbitMQ>>();

            // Create the event bus
            _eventBus = new EventBusRabbitMQ(
                connection.Object,
                _provider,
                logger.Object
            );

            // Subscribe the handler
            _eventBus.Subscribe<FakeEvent, FakeEventHandler>();

            // Store output helper
            _testOutput = output;
        }

        [Fact]
        public async Task ProcessEvent_Should_Invoke_Handler()
        {
            // Arrange
            var fakeEvent = new FakeEvent { Value = "Hello World" };
            var message = JsonConvert.SerializeObject(fakeEvent);

            _testOutput.WriteLine($"Message: {message}");
            _testOutput.WriteLine("----Before----");
            _testOutput.WriteLine($"_handler.WasHandled: {_handler.WasHandled}");
            _testOutput.WriteLine($"_handler.HandledValue: {_handler.HandledValue}");

            // Act
            var processEventMethod = typeof(EventBusRabbitMQ)
                .GetMethod("ProcessEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.NotNull(processEventMethod);

            await (Task)processEventMethod!.Invoke(_eventBus, new object[] { nameof(FakeEvent), message })!;
            _testOutput.WriteLine("----After----");
            _testOutput.WriteLine($"_handler.WasHandled: {_handler.WasHandled}");
            _testOutput.WriteLine($"_handler.HandledValue: {_handler.HandledValue}");

            // Assert
            Assert.True(_handler.WasHandled);
            Assert.Equal("Hello World", _handler.HandledValue);
        }
    }
}
