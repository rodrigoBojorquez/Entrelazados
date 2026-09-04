using DatePlanner.Domain.Dates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.ItineraryItems;

internal sealed class ItineraryItemConfiguration : IEntityTypeConfiguration<ItineraryItem>
{
    public void Configure(EntityTypeBuilder<ItineraryItem> builder)
    {
        builder.ToTable(
            "itinerary_items",
            table => table.HasCheckConstraint(
                "ck_itinerary_items_order_positive",
                "\"order\" > 0"));

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("DatePlanId")
            .HasColumnName("date_plan_id");

        builder.HasIndex("DatePlanId", nameof(ItineraryItem.Order))
            .IsUnique();

        builder.Property(item => item.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(
            item => item.Location,
            locationBuilder =>
            {
                locationBuilder.Property(location => location.Latitude)
                    .HasColumnName("latitude")
                    .HasPrecision(9, 6);

                locationBuilder.Property(location => location.Longitude)
                    .HasColumnName("longitude")
                    .HasPrecision(10, 6);
            });

        builder.Ignore(item => item.DomainEvents);
    }
}
