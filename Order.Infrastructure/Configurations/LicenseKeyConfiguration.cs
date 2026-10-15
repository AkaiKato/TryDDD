using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Configurations
{
    public class LicenseKeyConfiguration : IEntityTypeConfiguration<LicenseKeyRow>
    {
        public void Configure(EntityTypeBuilder<LicenseKeyRow> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Code).IsRequired();
            builder.Property(x => x.GameId).IsRequired();
            builder.Property(x => x.LicenseKeyStatus).IsRequired();
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}
