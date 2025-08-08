using Catalog.API.Data;
using Catalog.API.DTOs;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController(ProductContext context) : ControllerBase
{
    private readonly ProductContext _context = context;

    // GET: api/catalog/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetProductResponse>> GetProductById(Guid id)
    {
        var product = await _context.Catalog.FindAsync(id);
        if (product == null)
        {
            return NotFound($"Product with id '{id}' was not found." );
        }

        var result = new GetProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Brand = product.Brand,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            OnSale = product.OnSale,
            Stock = product.Stock,
            PictureUri = product.PictureUri,
            CreatedAt = product.CreatedAt
        };

        return Ok(result);
    }

    // POST: api/catalog
    [HttpPost]
    public async Task<ActionResult<GetProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Brand = request.Brand,
            Price = request.Price,
            DiscountedPrice = request.DiscountedPrice,
            Stock = request.Stock,
            PictureUri = request.PictureUri,
            CreatedAt = DateTime.UtcNow
        };

        _context.Catalog.Add(product);
        await _context.SaveChangesAsync();

        var result = new GetProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Brand = product.Brand,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            OnSale = product.OnSale,
            Stock = product.Stock,
            PictureUri = product.PictureUri,
            CreatedAt = product.CreatedAt
        };

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, result);
    }

    // PUT: api/catalog/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        if (id != request.Id)
            return BadRequest("Product ID mismatch between parameter and body.");

        var product = await _context.Catalog.FindAsync(request.Id);
        if (product == null)
        {
            return NotFound($"Product with id '{request.Id}' was not found." );
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Category = request.Category;
        product.Brand = request.Brand;
        product.Price = request.Price;
        product.DiscountedPrice = request.DiscountedPrice;
        product.Stock = request.Stock;
        product.PictureUri = request.PictureUri;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
