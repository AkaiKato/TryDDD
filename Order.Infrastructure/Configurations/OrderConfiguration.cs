using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderRow>
    {
        public void Configure(EntityTypeBuilder<OrderRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BuyerId).IsRequired();
            builder.Property(x => x.OrderStatus).IsRequired();
            builder.Property(x => x.CheckoutId).IsRequired();

            builder.HasIndex(x => x.CheckoutId).IsUnique();
        }
    }
}
