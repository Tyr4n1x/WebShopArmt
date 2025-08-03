using Basket.API.Repositories;
using Basket.API.Services;

namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();

            // Register the BasketRepository as a singleton
            builder.Services.AddSingleton<IBasketRepository, BasketRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<Services.BasketService>();
            app.MapGet("/", () => "Basket gRPC Service. Use a gRPC client to communicate.");

            app.Run();
        }
    }
}