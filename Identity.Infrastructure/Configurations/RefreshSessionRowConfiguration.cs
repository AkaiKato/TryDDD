using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Configurations
{
    public class RefreshSessionRowConfiguration : IEntityTypeConfiguration<RefreshSessionRow>
    {
        public void Configure(EntityTypeBuilder<RefreshSessionRow> builder)
        {
            builder.ToTable("RefreshSessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.TokenHash).IsRequired();
            builder.Property(x => x.Expiration).IsRequired();

            builder.HasIndex(x => x.TokenHash).IsUnique();
        }
    }
}
