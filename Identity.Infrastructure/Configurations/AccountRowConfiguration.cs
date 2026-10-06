using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Configurations
{
    public class AccountRowConfiguration : IEntityTypeConfiguration<AccountRow>
    {
        public void Configure(EntityTypeBuilder<AccountRow> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
