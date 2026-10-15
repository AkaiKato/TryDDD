using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Configurations
{
    public class GameSnapshotConfiguration : IEntityTypeConfiguration<GameSnapshot>
    {
        public void Configure(EntityTypeBuilder<GameSnapshot> builder)
        {
            builder.HasKey(x => x.GameId);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.ForSale).IsRequired();
        }
    }
}
