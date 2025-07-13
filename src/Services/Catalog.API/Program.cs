using Microsoft.EntityFrameworkCore;
using Catalog.API.Data;

namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configures the DbContextFactory for the ProductContext with a InMemory database (for now)
            builder.Services.AddDbContext<ProductContext>(opt =>
                opt.UseInMemoryDatabase("ProductDatabase"));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
