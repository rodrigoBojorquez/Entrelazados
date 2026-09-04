using DatePlanner.Infrastructure.DateIdeas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.DateIdeaItineraryItems;

internal sealed class DateIdeaItineraryItemConfiguration : IEntityTypeConfiguration<DateIdeaItineraryItem>
{
    public void Configure(EntityTypeBuilder<DateIdeaItineraryItem> builder)
    {
        builder.ToTable(
            "date_idea_itinerary_items",
            table => table.HasCheckConstraint(
                "ck_date_idea_itinerary_items_order_positive",
                "\"order\" > 0"));

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("DateIdeaId")
            .HasColumnName("date_idea_id");

        builder.HasIndex("DateIdeaId", nameof(DateIdeaItineraryItem.Order))
            .IsUnique();

        builder.Property(item => item.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Tags)
            .HasColumnType("text[]")
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
    }
}
