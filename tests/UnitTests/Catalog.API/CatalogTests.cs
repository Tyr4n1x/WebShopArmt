using Catalog.API;
using Catalog.API.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace UnitTests.Catalog.API
{
    public class CatalogTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _testOutput;

        private readonly CreateProductRequest sampleProduct = new()
        {
            Name = "Test Product",
            Description = "A product used for testing.",
            Category = "Test Category",
            Brand = "Test Brand",
            Price = 49.99m,
            DiscountedPrice = 29.99m,
            Stock = 10,
            PictureUri = "https://example.com/product.jpg"
        };

        public CatalogTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _testOutput = output;
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedResult_ForValidInput()
        {
            var response = await _client.PostAsJsonAsync("/api/catalog", sampleProduct);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_ForMissingName()
        {
            var invalidProduct = new CreateProductRequest
            {
                Description = "Product description",
                Category = "Test",
                Price = 19.99m
            };

            var response = await _client.PostAsJsonAsync("/api/catalog", invalidProduct);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_ForMissingDescription()
        {
            var invalidProduct = new CreateProductRequest
            {
                Name = "Product name",
                Category = "Test",
                Price = 19.99m
            };

            var response = await _client.PostAsJsonAsync("/api/catalog", invalidProduct);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_ForMissingCategory()
        {
            var invalidProduct = new CreateProductRequest
            {
                Name = "Product name",
                Description = "Product description",
                Price = 19.99m
            };

            var response = await _client.PostAsJsonAsync("/api/catalog", invalidProduct);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_ForMissingPrice()
        {
            var invalidProduct = new CreateProductRequest
            {
                Name = "Product name",
                Description = "Product description",
                Category = "Test"
            };

            var response = await _client.PostAsJsonAsync("/api/catalog", invalidProduct);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetProductById_ReturnsOk_ForExistingProduct()
        {
            // First create a product
            var createResponse = await _client.PostAsJsonAsync("/api/catalog", sampleProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<GetProductResponse>();

            // Now get the created product by ID
            var getResponse = await _client.GetAsync($"/api/catalog/{createdProduct!.Id}");

            _testOutput.WriteLine($"Response: {getResponse.StatusCode} - {await getResponse.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

            var result = await getResponse.Content.ReadFromJsonAsync<GetProductResponse>();
            Assert.Equal(sampleProduct.Name, result!.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_ForUnknownId()
        {
            var fakeId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/catalog/{fakeId}");

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_ForValidUpdate()
        {
            // Create a product first
            var createResponse = await _client.PostAsJsonAsync("/api/catalog", sampleProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<GetProductResponse>();

            var updateRequest = new UpdateProductRequest
            {
                Id = createdProduct!.Id,
                Name = "Updated name",
                Description = "Updated description",
                Category = "Updated category",
                Brand = "Updated brand",
                Price = 59.99m,
                DiscountedPrice = 49.99m,
                Stock = 5,
                PictureUri = "https://example.com/updated_product.jpg"
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/catalog/{createdProduct.Id}", updateRequest);

            _testOutput.WriteLine($"Response: {updateResponse.StatusCode}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);

            var result = await _client.GetFromJsonAsync<GetProductResponse>($"/api/catalog/{createdProduct.Id}");
            Assert.Equal("Updated name", result!.Name);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_ForMismatchedId()
        {
            var request = new UpdateProductRequest
            {
                Id = Guid.NewGuid(),
                Name = "Mismatch name",
                Description = "Mismatch description",
                Category = "Mismatch category",
                Price = 99.99m
            };

            var response = await _client.PutAsJsonAsync("/api/catalog/" + Guid.NewGuid(), request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_ForUnknownProduct()
        {
            var id = Guid.NewGuid();
            var request = new UpdateProductRequest
            {
                Id = id,
                Name = "Unknown name",
                Description = "Unknown description",
                Category = "Unknown category",
                Price = 10.0m
            };

            var response = await _client.PutAsJsonAsync($"/api/catalog/{id}", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
