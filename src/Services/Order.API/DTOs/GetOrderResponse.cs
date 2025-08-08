using Order.API.Models;

namespace Order.API.DTOs
{
    public class GetOrderResponse
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = [];
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
