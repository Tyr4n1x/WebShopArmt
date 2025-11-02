using EventBus.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Order.API;
using Order.API.DTOs;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace UnitTests.Order.API
{
    public class PaymentTests : IClassFixture<WebApplicationFactory<Program>>
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

        public PaymentTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _testOutput = output;
        }

        [Fact]
        public async Task CreatePaymentIntent_ReturnsOk_ForValidOrder()
        {
            // Create order
            var orderResponse = await _client.PostAsJsonAsync("/api/orders", sampleOrder);
            var order = await orderResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

            // Create payment intent
            var response = await _client.PostAsync($"/api/payments/{order!.Id}", null);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreatePaymentIntent_ReturnsNotFound_ForUnknownOrder()
        {
            // Create payment intent with unknown GUID
            var unknownId = Guid.NewGuid();
            var response = await _client.PostAsync($"/api/payments/{unknownId}", null);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void EventBus_IsRegistered()
        {
            using var appFactory = new WebApplicationFactory<Program>();
            using var scope = appFactory.Services.CreateScope();

            var eventBus = scope.ServiceProvider.GetService<IEventBus>();

            Assert.NotNull(eventBus);
        }

    }
}
