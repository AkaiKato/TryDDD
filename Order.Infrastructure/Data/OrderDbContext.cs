using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Configurations;
using MassTransit;

namespace Order.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public DbSet<OrderRow> Orders { get; set; }
        public DbSet<OrderLineRow> OrderLines { get; set; }
        public DbSet<LicenseKeyRow> LicenseKeyRows { get; set; }
        public DbSet<GameSnapshot> GameSnapshots { get; set; }
        public DbSet<RejectedCheckoutRow> RejectedCheckouts { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddInboxStateEntity();
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseKeyConfiguration());
            modelBuilder.ApplyConfiguration(new GameSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new RejectedCheckoutConfiguration());
        }
    }
}
