using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs
{
    public class UpdateProductRequest : CreateProductRequest
    {
        [Required]
        public required Guid Id { get; set; }
    }
}
