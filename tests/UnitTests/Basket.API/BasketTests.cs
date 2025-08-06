using Basket.API;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit.Abstractions;
using static Basket.API.GrpcBasketService;

namespace UnitTests.Basket.API
{
    public class BasketTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly GrpcChannel _channel;
        private readonly GrpcBasketServiceClient _client;
        private readonly ITestOutputHelper _testOutput;

        public BasketTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            var client = factory.CreateDefaultClient(); // HTTP client from factory
            _channel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions { HttpClient = client });
            _client = new GrpcBasketServiceClient(_channel);
            _testOutput = output;
        }

        [Fact]
        public async Task UpdateBasket_ReturnsUpdatedBasket()
        {
            var request = new UpdateBasketRequest
            {
                CustomerId = "test-user-1"
            };
            request.Items.Add(new GrpcBasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                UnitPrice = 9.99,
                Quantity = 2,
                PictureUri = "https://example.com/image.jpg"
            });
            request.Items.Add(new GrpcBasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product 2",
                UnitPrice = 15.99,
                Quantity = 1,
                PictureUri = "https://example.com/image2.jpg"
            });

            var response = await _client.UpdateBasketAsync(request);

            Assert.Equal(request.CustomerId, response.CustomerId);
            for (int i = 0; i < request.Items.Count; i++)
            {
                Assert.Equal(request.Items[i].Id, response.Items[i].Id);
                Assert.Equal(request.Items[i].Name, response.Items[i].Name);
                Assert.Equal(request.Items[i].UnitPrice, response.Items[i].UnitPrice);
                Assert.Equal(request.Items[i].Quantity, response.Items[i].Quantity);
                Assert.Equal(request.Items[i].PictureUri, response.Items[i].PictureUri);
            }
            _testOutput.WriteLine($"Request: {request}");
            _testOutput.WriteLine($"Response: {response}");
        }

        [Fact]
        public async Task UpdateBasket_ThrowsRpcException_WhenRequestIsNull()
        {
            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await _client.UpdateBasketAsync(null);
            });

            _testOutput.WriteLine($"Exception: {ex.Message}");
        }

        [Fact]
        public async Task UpdateBasket_ThrowsRpcException_WhenCustomerIdIsMissing()
        {
            var request = new UpdateBasketRequest
            {
                CustomerId = ""
            };
            request.Items.Add(new GrpcBasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                UnitPrice = 9.99,
                Quantity = 2,
                PictureUri = "https://example.com/image.jpg"
            });

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await _client.UpdateBasketAsync(request);
            });

            _testOutput.WriteLine($"Exception: {ex.Message}");
        }

        [Fact]
        public async Task UpdateBasket_ThrowsRpcException_WhenItemsIsEmpty()
        {
            var request = new UpdateBasketRequest
            {
                CustomerId = "test-user-2"
            };

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await _client.UpdateBasketAsync(request);
            });

            _testOutput.WriteLine($"Exception: {ex.Message}");
        }

        [Fact]
        public async Task UpdateBasket_ShouldOverwriteExistingBasket()
        {
            var customerId = "test-user-3";

            var firstRequest = new UpdateBasketRequest
            {
                CustomerId = customerId
            };
            firstRequest.Items.Add(new GrpcBasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                UnitPrice = 9.99,
                Quantity = 2,
                PictureUri = "https://example.com/image.jpg"
            });

            var secondRequest = new UpdateBasketRequest
            {
                CustomerId = customerId
            };
            secondRequest.Items.Add(new GrpcBasketItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product 2",
                UnitPrice = 15.99,
                Quantity = 1,
                PictureUri = "https://example.com/image2.jpg"
            });

            var firstResponse = await _client.UpdateBasketAsync(firstRequest);
            var secondResponse = await _client.UpdateBasketAsync(secondRequest);

            Assert.Single(secondResponse.Items);

            _testOutput.WriteLine($"firstRequest: {firstRequest}");
            _testOutput.WriteLine($"firstResponse: {firstResponse}");
            _testOutput.WriteLine($"secondRequest: {secondRequest}");
            _testOutput.WriteLine($"secondResponse: {secondResponse}");
        }

        [Fact]
        public async Task GetBasket_ShouldReturnBasket_WhenExists()
        {
            var customerId = "test-user-3";

            var request = new UpdateBasketRequest
            {
                CustomerId = customerId,
                Items = {
                    new GrpcBasketItem {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Test Product",
                        UnitPrice = 9.99,
                        Quantity = 2,
                        PictureUri = "https://example.com/image.jpg"
                    }
                }
            };

            // First insert
            await _client.UpdateBasketAsync(request);

            // Then retrieve
            var response = await _client.GetBasketAsync(new GetBasketRequest { CustomerId = customerId });

            Assert.Equal(request.CustomerId, response.CustomerId);
            for (int i = 0; i < request.Items.Count; i++)
            {
                Assert.Equal(request.Items[i].Id, response.Items[i].Id);
                Assert.Equal(request.Items[i].Name, response.Items[i].Name);
                Assert.Equal(request.Items[i].UnitPrice, response.Items[i].UnitPrice);
                Assert.Equal(request.Items[i].Quantity, response.Items[i].Quantity);
                Assert.Equal(request.Items[i].PictureUri, response.Items[i].PictureUri);
            }
            _testOutput.WriteLine($"Request: {request}");
            _testOutput.WriteLine($"Response: {response}");
        }

        [Fact]
        public async Task GetBasket_ShouldReturnEmpty_WhenBasketNotFound()
        {
            var customerId = "test-user-4";

            var response = await _client.GetBasketAsync(new GetBasketRequest { CustomerId = customerId });

            Assert.Equal(customerId, response.CustomerId);
            Assert.Empty(response.Items);

            _testOutput.WriteLine($"Response: {response}");
        }


        [Fact]
        public async Task DeleteBasket_ShouldReturnSuccess()
        {
            var customerId = "test-user-5";

            var request = new UpdateBasketRequest
            {
                CustomerId = customerId,
                Items = {
                    new GrpcBasketItem {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Test Product",
                        UnitPrice = 9.99,
                        Quantity = 2,
                        PictureUri = "https://example.com/image.jpg"
                    }
                }
            };

            // First insert
            await _client.UpdateBasketAsync(request);

            // Delete
            var deleteResult = await _client.DeleteBasketAsync(new DeleteBasketRequest { CustomerId = customerId });

            Assert.True(deleteResult.Success);

            _testOutput.WriteLine($"deleteResult: {deleteResult}");

            // Confirm deletion
            var getResponse = await _client.GetBasketAsync(new GetBasketRequest { CustomerId = customerId });

            Assert.Equal(customerId, getResponse.CustomerId);
            Assert.Empty(getResponse.Items);

            _testOutput.WriteLine($"getResponse: {getResponse}");
        }

        [Fact]
        public async Task DeleteBasket_ShouldReturnFalse_WhenBasketDoesNotExist()
        {
            var customerId = "test-user-6";

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await _client.DeleteBasketAsync(new DeleteBasketRequest { CustomerId = customerId });
            });

            _testOutput.WriteLine($"Exception: {ex.Message}");
        }

    }
}
