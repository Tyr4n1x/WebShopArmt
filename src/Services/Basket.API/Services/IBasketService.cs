using Basket.API.Models;

namespace Basket.API.Services
{
    public interface IBasketService
    {
        Task<CustomerBasket?> GetBasketAsync(string customerId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string customerId);
    }
}
