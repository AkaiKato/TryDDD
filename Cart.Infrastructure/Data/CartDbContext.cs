using Cart.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Data
{
    public class CartDbContext : DbContext
    {
        public DbSet<CartRow> Carts { get; set; }
        public DbSet<CartItemRow> CartItem { get; set; }
        public DbSet<GameSnapshot> GameSnapshots { get; set; }

        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new GameSnapshotConfiguration());
        }
    }
}
