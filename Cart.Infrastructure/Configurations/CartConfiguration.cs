using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Infrastructure.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<CartRow>
    {
        public void Configure(EntityTypeBuilder<CartRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.BuyerId).IsUnique();
            builder.Property(x => x.BuyerId).IsRequired();
            builder.Property(x => x.CartStatus).IsRequired();
        }
    }
}
