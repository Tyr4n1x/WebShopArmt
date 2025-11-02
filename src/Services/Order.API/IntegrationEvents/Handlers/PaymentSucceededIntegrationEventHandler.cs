using EventBus.Events;
using EventBus.Interfaces;
using Order.API.Data;

namespace Order.API.IntegrationEvents.Handlers
{
    public class PaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<PaymentSucceededIntegrationEvent>
    {
        private readonly OrderContext _context;
        private readonly ILogger<PaymentSucceededIntegrationEventHandler> _logger;

        public PaymentSucceededIntegrationEventHandler(OrderContext context, ILogger<PaymentSucceededIntegrationEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task HandleAsync(PaymentSucceededIntegrationEvent @event)
        {
            var order = await _context.Orders.FindAsync(@event.OrderId);
            if (order is null)
            {
                _logger.LogWarning("Order not found for ID {OrderId}", @event.OrderId);
                return;
            }

            order.PaymentStatus = "Succeeded";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated order {OrderId} payment status to 'Succeeded'.", @event.OrderId);
        }
    }
}
