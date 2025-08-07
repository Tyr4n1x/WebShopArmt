namespace Order.API.Models
{
    public class CustomerOrder(string customerId)
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = customerId;
        public List<OrderItem> Items { get; set; } = [];
        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
