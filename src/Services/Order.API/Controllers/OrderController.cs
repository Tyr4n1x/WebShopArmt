using Microsoft.AspNetCore.Mvc;
using Order.API.Data;
using Order.API.DTOs;
using Order.API.Models;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(OrderContext context) : ControllerBase
{
    private readonly OrderContext _context = context;

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetOrderResponse>> GetOrderById(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null)
        {
            return NotFound($"Order with id '{id}' not found." );
        }

        var response = new GetOrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Items = order.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                PictureUri = i.PictureUri
            }).ToList(),
            Total = order.Total,
            CreatedAt = order.CreatedAt
        };

        return Ok(response);
    }


    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<GetOrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = new CustomerOrder(request.CustomerId)
        {
            Id = Guid.NewGuid(),
            Items = request.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                PictureUri = i.PictureUri
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var response = new GetOrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Items = order.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                PictureUri = i.PictureUri
            }).ToList(),
            Total = order.Total,
            CreatedAt = order.CreatedAt
        };

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
    }
}
