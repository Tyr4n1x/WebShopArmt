using Order.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Order.API.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();
    }
}
