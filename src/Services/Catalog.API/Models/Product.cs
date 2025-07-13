namespace Catalog.API.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public decimal Price { get; set; } = 0.01m;

        public decimal? DiscountedPrice { get; set; }

        public bool OnSale => DiscountedPrice.HasValue && DiscountedPrice.Value < Price;

        public int? Stock { get; set; }

        public string? PictureUri { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
