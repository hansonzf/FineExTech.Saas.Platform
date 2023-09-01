using LocationApi.Domain.AggregateModels.LocationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocationApi.Infrastructure.EntityConfigurations
{
    public class LocationEntityTypeConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.Owner).IsRequired();
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(30);
            builder.OwnsOne(e => e.Addr, addr =>
            {
                addr.Property(ad => ad.Country).HasDefaultValue("CN").HasColumnName("Country").HasMaxLength(3);
                addr.Property(ad => ad.Province).HasColumnName("Province").HasMaxLength(20);
                addr.Property(ad => ad.City).HasColumnName("City").HasMaxLength(10);
                addr.Property(ad => ad.District).HasColumnName("District").HasMaxLength(10);
                addr.Property(ad => ad.PostalCode).IsRequired(false).HasColumnName("PostalCode").HasMaxLength(5);
                addr.Property(ad => ad.DetailAddress).HasColumnName("DetailAddress").HasMaxLength(200);
            });

            builder.HasIndex(e => e.Owner).IsClustered(true);
        }
    }
}
