namespace Catalog.API.DTOs
{
    public class GetProductResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public decimal Price { get; set; } = 0.0m;

        public decimal? DiscountedPrice { get; set; }

        public bool OnSale { get; set; }

        public int? Stock { get; set; }

        public string? PictureUri { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
