using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DatingPreferencesEntity = DatePlanner.Domain.Users.DatingPreferences;

namespace DatePlanner.Infrastructure.DatingPreferences;

internal sealed class DatingPreferencesConfiguration : IEntityTypeConfiguration<DatingPreferencesEntity>
{
    public void Configure(EntityTypeBuilder<DatingPreferencesEntity> builder)
    {
        builder.ToTable("dating_preferences");

        builder.HasKey(preferences => preferences.Id);

        builder.Property(preferences => preferences.Id)
            .ValueGeneratedNever();

        builder.HasOne<User>()
            .WithOne(user => user.DatingPreferences)
            .HasForeignKey<DatingPreferencesEntity>(preferences => preferences.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(preferences => preferences.UserId)
            .IsUnique();

        builder.Ignore(preferences => preferences.FavoriteActivities);
        builder.Ignore(preferences => preferences.AvoidedActivities);
        builder.Ignore(preferences => preferences.FoodPreferences);

        builder.Property<List<string>>("_favoriteActivities")
            .HasColumnName("favorite_activities")
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property<List<string>>("_avoidedActivities")
            .HasColumnName("avoided_activities")
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property<List<string>>("_foodPreferences")
            .HasColumnName("food_preferences")
            .HasColumnType("text[]")
            .IsRequired();

        builder.Ignore(preferences => preferences.DomainEvents);
    }
}
