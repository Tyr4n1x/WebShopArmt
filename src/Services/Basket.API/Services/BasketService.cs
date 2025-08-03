using Basket.API.Models;
using Basket.API.Repositories;
using Grpc.Core;

namespace Basket.API.Services;

public class BasketService(IBasketRepository repository) : Basket.API.BasketService.BasketServiceBase
{
    private readonly IBasketRepository _repository = repository;

    public override async Task<BasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var basket = await _repository.GetBasketAsync(request.CustomerId);

        if (basket is null)
        {
            // Return an empty basket
            return new BasketResponse
            {
                CustomerId = request.CustomerId
            };
        }

        var response = new BasketResponse
        {
            CustomerId = basket.CustomerId
        };

        response.Items.AddRange(basket.Items.Select(item => new Basket.API.BasketItem
        {
            Id = item.Id.ToString(), // Convert Guid to string
            Name = item.Name,
            UnitPrice = (double)item.UnitPrice,
            Quantity = item.Quantity,
            PictureUri = item.PictureUri ?? string.Empty
        }));

        return response;
    }

    public override async Task<BasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
    {
        var updatedBasket = new CustomerBasket(request.CustomerId)
        {
            Items = request.Items.Select(item => new Models.BasketItem
            {
                Id = Guid.TryParse(item.Id, out Guid guid) ? guid : Guid.NewGuid(),
                Name = item.Name,
                UnitPrice = (decimal)item.UnitPrice,
                Quantity = item.Quantity,
                PictureUri = string.IsNullOrWhiteSpace(item.PictureUri) ? null : item.PictureUri
            }).ToList()
        };

        var savedBasket = await _repository.UpdateBasketAsync(updatedBasket);

        var response = new BasketResponse
        {
            CustomerId = savedBasket.CustomerId
        };

        response.Items.AddRange(savedBasket.Items.Select(item => new Basket.API.BasketItem
        {
            Id = item.Id.ToString(), // Convert Guid to string
            Name = item.Name,
            UnitPrice = (double)item.UnitPrice,
            Quantity = item.Quantity,
            PictureUri = item.PictureUri ?? string.Empty
        }));

        return response;
    }

    public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        var result = await _repository.DeleteBasketAsync(request.CustomerId);

        return new DeleteBasketResponse
        {
            Success = result
        };
    }
}
