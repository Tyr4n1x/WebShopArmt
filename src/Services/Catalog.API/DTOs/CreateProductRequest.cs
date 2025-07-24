using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [Required]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discounted price must be greater than or equal to 0.")]
        public decimal? DiscountedPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        [Url]
        public string? PictureUri { get; set; }
    }
}
