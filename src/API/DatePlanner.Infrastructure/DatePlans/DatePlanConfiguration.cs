using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.DatePlans;

internal sealed class DatePlanConfiguration : IEntityTypeConfiguration<DatePlan>
{
    public void Configure(EntityTypeBuilder<DatePlan> builder)
    {
        builder.ToTable("date_plans");

        builder.HasKey(datePlan => datePlan.Id);

        builder.Property(datePlan => datePlan.Id)
            .ValueGeneratedNever();

        builder.HasOne<Couple>()
            .WithMany()
            .HasForeignKey(datePlan => datePlan.CoupleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(datePlan => datePlan.PartnerWhoPlannedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(datePlan => datePlan.CoupleId);
        builder.HasIndex(datePlan => datePlan.PartnerWhoPlannedId);

        builder.Property(datePlan => datePlan.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(datePlan => datePlan.Status)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();

        builder.HasMany(datePlan => datePlan.Itinerary)
            .WithOne()
            .HasForeignKey("DatePlanId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(datePlan => datePlan.Itinerary)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(datePlan => datePlan.DomainEvents);
    }
}
