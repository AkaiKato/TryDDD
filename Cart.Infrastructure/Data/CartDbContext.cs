using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
