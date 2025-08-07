namespace Order.API.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } = 0.01m;
        public int Quantity { get; set; } = 0;
        public string? PictureUri { get; set; }
    }
}
