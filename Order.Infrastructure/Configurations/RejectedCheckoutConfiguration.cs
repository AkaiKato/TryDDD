using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Configurations
{
    public class RejectedCheckoutConfiguration : IEntityTypeConfiguration<RejectedCheckoutRow>
    {
        public void Configure(EntityTypeBuilder<RejectedCheckoutRow> builder)
        {
            builder.HasKey(x => x.CheckoutId);
        }
    }
}
