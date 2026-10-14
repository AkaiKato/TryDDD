using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<GameRow>
    {
        public void Configure(EntityTypeBuilder<GameRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.GameStatus).IsRequired();
        }
    }
}
