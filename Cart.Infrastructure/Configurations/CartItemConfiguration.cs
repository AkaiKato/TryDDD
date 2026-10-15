using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Infrastructure.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItemRow>
    {
        public void Configure(EntityTypeBuilder<CartItemRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.CartId, x.GameId }).IsUnique();

            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(120);

            builder.HasOne(x => x.Cart)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
