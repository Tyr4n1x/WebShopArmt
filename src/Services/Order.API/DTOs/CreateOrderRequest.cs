using Order.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Order.API.DTOs
{
    public class CreateOrderItem
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 9999.99)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? PictureUri { get; set; }
    }

    public class CreateOrderRequest
    {
        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<CreateOrderItem> Items { get; set; } = [];
    }
}
