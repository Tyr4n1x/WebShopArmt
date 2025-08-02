namespace Basket.API.Models
{
    public class CustomerBasket(string customerId)
    {
        public string CustomerId { get; set; } = customerId;
        public List<BasketItem> Items { get; set; } = [];
    }
}
