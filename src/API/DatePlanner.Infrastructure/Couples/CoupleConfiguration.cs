using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.Couples;

internal sealed class CoupleConfiguration : IEntityTypeConfiguration<Couple>
{
    public void Configure(EntityTypeBuilder<Couple> builder)
    {
        builder.ToTable(
            "couples",
            table => table.HasCheckConstraint(
                "ck_couples_different_partners",
                "partner_one_id <> partner_two_id"));

        builder.HasKey(couple => couple.Id);

        builder.Property(couple => couple.Id)
            .ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(couple => couple.PartnerOneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(couple => couple.PartnerTwoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(couple => couple.PartnerOneId);
        builder.HasIndex(couple => couple.PartnerTwoId);

        builder.Property(couple => couple.BeginRelationshipAt)
            .HasColumnType("timestamp with time zone");

        builder.Ignore(couple => couple.DomainEvents);
    }
}
