using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Configurations
{
    public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLineRow>
    {
        public void Configure(EntityTypeBuilder<OrderLineRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.OrderId, x.GameId }).IsUnique();

            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.Title).IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.OrderLines)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
