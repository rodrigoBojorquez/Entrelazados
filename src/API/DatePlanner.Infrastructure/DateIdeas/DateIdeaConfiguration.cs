using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.DateIdeas;

internal sealed class DateIdeaConfiguration : IEntityTypeConfiguration<DateIdea>
{
    public void Configure(EntityTypeBuilder<DateIdea> builder)
    {
        builder.ToTable(
            "date_ideas",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_date_ideas_preferred_budget_non_negative",
                    "preferred_budget >= 0");
                table.HasCheckConstraint(
                    "ck_date_ideas_estimated_budget_non_negative",
                    "estimated_budget IS NULL OR estimated_budget >= 0");
            });

        builder.HasKey(dateIdea => dateIdea.Id);

        builder.Property(dateIdea => dateIdea.Id)
            .ValueGeneratedNever();

        builder.HasOne<Couple>()
            .WithMany()
            .HasForeignKey(dateIdea => dateIdea.CoupleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DatePlan>()
            .WithMany()
            .HasForeignKey(dateIdea => dateIdea.ResultPlan)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(dateIdea => dateIdea.ResultPlan)
            .HasColumnName("result_plan_id");

        builder.HasIndex(dateIdea => dateIdea.CoupleId);
        builder.HasIndex(dateIdea => dateIdea.ResultPlan);

        builder.Property(dateIdea => dateIdea.PreferredBudget)
            .HasPrecision(12, 2);

        builder.Property(dateIdea => dateIdea.EstimatedBudget)
            .HasPrecision(12, 2);

        builder.Property(dateIdea => dateIdea.City)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(dateIdea => dateIdea.CreatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasMany(dateIdea => dateIdea.Itinerary)
            .WithOne()
            .HasForeignKey("DateIdeaId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
