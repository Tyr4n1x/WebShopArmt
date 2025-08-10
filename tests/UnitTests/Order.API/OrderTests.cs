using Order.API;
using Order.API.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace UnitTests.Order.API
{
    public class OrderTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _testOutput;

        private readonly CreateOrderRequest sampleOrder = new()
        {
            CustomerId = "test-customer-1",
            Items =
            [
                new CreateOrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product 1",
                    UnitPrice = 49.99m,
                    Quantity = 2,
                    PictureUri = "https://example.com/product1.jpg"
                },
                new CreateOrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product 2",
                    UnitPrice = 19.99m,
                    Quantity = 1,
                    PictureUri = "https://example.com/product2.jpg"
                }
            ]
        };

        public OrderTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _testOutput = output;
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedResult_ForValidInput()
        {
            var response = await _client.PostAsJsonAsync("/api/orders", sampleOrder);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_ForMissingCustomerId()
        {
            var invalidOrder = new CreateOrderRequest
            {
                Items = sampleOrder.Items
            };

            var response = await _client.PostAsJsonAsync("/api/orders", invalidOrder);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_ForEmptyItems()
        {
            var invalidOrder = new CreateOrderRequest
            {
                CustomerId = sampleOrder.CustomerId,
                Items = []
            };

            var response = await _client.PostAsJsonAsync("/api/orders", invalidOrder);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_ForExistingOrder()
        {
            // First create an order
            var createResponse = await _client.PostAsJsonAsync("/api/orders", sampleOrder);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

            // Now get the created order by ID
            var getResponse = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");

            _testOutput.WriteLine($"Response: {getResponse.StatusCode} - {await getResponse.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.OK, getResponse.StatusCode);

            var result = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();
            Assert.Equal(sampleOrder.CustomerId, result!.CustomerId);
            Assert.Equal(sampleOrder.Items.Count, result.Items.Count);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_ForUnknownCustomerId()
        {
            // First create an order
            var createResponse = await _client.PostAsJsonAsync("/api/orders", sampleOrder);

            // Now try to get an order with an unknown ID
            var unknownId = "test-customer-10";
            var getResponse = await _client.GetAsync($"/api/orders/{unknownId}");

            _testOutput.WriteLine($"Response: {getResponse.StatusCode} - {await getResponse.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
