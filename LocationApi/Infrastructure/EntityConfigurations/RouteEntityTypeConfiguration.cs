using LocationApi.Domain.AggregateModels.RouteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Route = LocationApi.Domain.AggregateModels.RouteAggregate.Route;

namespace LocationApi.Infrastructure.EntityConfigurations
{
    public class RouteEntityTypeConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.RouteName).HasMaxLength(50);
            builder.Property(e => e.Distance).IsRequired();
            builder.OwnsOne(e => e.Origin, o =>
            {
                o.Property(l => l.LocationId).IsRequired().HasColumnName("OriginLocationId");
                o.Property(l => l.LocationName).IsRequired().HasMaxLength(100).HasColumnName("OriginLocationName");
                o.Property(l => l.City).HasColumnName("OriginCity");
            });
            builder.OwnsOne(e => e.Destination, o =>
            {
                o.Property(l => l.LocationId).IsRequired().HasColumnName("DestinationLocationId");
                o.Property(l => l.LocationName).IsRequired().HasMaxLength(100).HasColumnName("DestinationLocationName");
                o.Property(l => l.City).HasColumnName("DestinationCity");
            });
            builder.Ignore(e => e.Segments);
            builder.Ignore(e => e.Legs);

            builder.HasIndex(e => e.OwnerId).IsClustered(true);
        }
    }

    public class SegmentEntityTypeConfiguration : IEntityTypeConfiguration<Segment>
    {
        public void Configure(EntityTypeBuilder<Segment> builder)
        {
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.Property(e => e.RouteId).IsRequired();
            builder.Property(e => e.Index).IsRequired();
            builder.OwnsOne(e => e.From, o =>
            {
                o.Property(l => l.LocationId).IsRequired().HasColumnName("FromLocationId");
                o.Property(l => l.LocationName).IsRequired().HasMaxLength(100).HasColumnName("FromLocationName");
                o.Property(l => l.City).HasColumnName("FromCity");
            });
            builder.OwnsOne(e => e.To, o =>
            {
                o.Property(l => l.LocationId).IsRequired().HasColumnName("ToLocationId");
                o.Property(l => l.LocationName).IsRequired().HasMaxLength(100).HasColumnName("ToLocationName");
                o.Property(l => l.City).HasColumnName("ToCity");
            });
            builder.Property(e => e.Distance)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasIndex(e => e.RouteId).IsClustered(true);
        }
    }
}
