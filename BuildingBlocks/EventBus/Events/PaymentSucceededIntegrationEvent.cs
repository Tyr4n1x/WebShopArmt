namespace EventBus.Events
{
    public class PaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }

        public PaymentSucceededIntegrationEvent(Guid orderId, decimal amount)
        {
            OrderId = orderId;
            Amount = amount;
        }
    }
}