using Basket.API.Models;
using System.Collections.Concurrent;

namespace Basket.API.Services
{
    public class BasketService : IBasketService
    {
        private readonly ConcurrentDictionary<string, CustomerBasket> _baskets = new();

        public Task<CustomerBasket?> GetBasketAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("CustomerId cannot be null or empty.", nameof(customerId));

            if (!_baskets.ContainsKey(customerId))
                return Task.FromResult<CustomerBasket?>(null);

            _baskets.TryGetValue(customerId, out var basket);
            return Task.FromResult(basket);
        }
        public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            if (basket == null)
                throw new ArgumentNullException(nameof(basket));

            if (string.IsNullOrWhiteSpace(basket.CustomerId))
                throw new ArgumentException("CustomerId cannot be null or empty.", nameof(basket.CustomerId));

            if (basket.Items == null)
                throw new ArgumentException("Items cannot be null.", nameof(basket.Items));

            if (basket.Items.Count == 0)
                throw new ArgumentException("Basket must contain at least one item.", nameof(basket.Items));


            _baskets[basket.CustomerId] = basket;
            return Task.FromResult(basket);
        }
        public Task<bool> DeleteBasketAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("CustomerId cannot be null or empty.", nameof(customerId));

            if (!_baskets.ContainsKey(customerId))
                throw new ArgumentException("Basket does not exist for the given CustomerId.", nameof(customerId));

            return Task.FromResult(_baskets.TryRemove(customerId, out _));
        }
    }
}
