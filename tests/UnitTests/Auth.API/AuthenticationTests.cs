using Auth.API;
using Auth.API.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace UnitTests.Auth.API
{
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _testOutput;

        private readonly Dictionary<string, string> sampleUser = new()
        {
            { "Email", "user@example.com" },
            { "Password", "Password123!" }
        };

        public AuthenticationTests(WebApplicationFactory<Program> factory, ITestOutputHelper _testOutput)
        {
            _client = factory.CreateClient();
            this._testOutput = _testOutput;
        }  
            

        [Fact]
        public async Task Register_ReturnsSuccess_ForValidInput()
        {
            var request = new RegisterRequest
            {
                Email = sampleUser["Email"],
                Password = sampleUser["Password"],
                ConfirmPassword = sampleUser["Password"]
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/register", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_ForMismatchedPasswords()
        {
            var request = new RegisterRequest
            {
                Email = sampleUser["Email"],
                Password = sampleUser["Password"],
                ConfirmPassword = "WrongPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/register", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_ForWrongEmail()
        {
            var request = new RegisterRequest
            {
                Email = "user",
                Password = sampleUser["Password"],
                ConfirmPassword = sampleUser["Password"]
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/register", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_ForMissingEmail()
        {
            var request = new RegisterRequest
            {
                FirstName = "user",
                Password = sampleUser["Password"],
                ConfirmPassword = sampleUser["Password"]
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/register", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsSuccess_ForValidInput()
        {
            var request = new LoginRequest
            {
                Entry = sampleUser["Email"],
                Password = sampleUser["Password"]
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_ForWrongEntry()
        {
            var request = new LoginRequest
            {
                Entry = "test@example.com",
                Password = sampleUser["Password"]
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_ForWrongPassword()
        {
            var request = new LoginRequest
            {
                Entry = sampleUser["Email"],
                Password = "Password124!"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", request);

            _testOutput.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
