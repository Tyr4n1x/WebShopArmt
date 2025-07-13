using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Description { get; set; }

        [Required]
        [StringLength(100)]
        public required string Category { get; set; }

        [StringLength(100)]
        public string? Brand { get; set; }

        [Required]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be greater than 0.")]
        public required decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discounted price must be greater than or equal to 0.")]
        public decimal? DiscountedPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        [Url]
        public string? PictureUri { get; set; }
    }
}
