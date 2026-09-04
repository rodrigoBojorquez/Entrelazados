using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.TurnGameRounds;

internal sealed class TurnGameRoundConfiguration : IEntityTypeConfiguration<TurnGameRound>
{
    public void Configure(EntityTypeBuilder<TurnGameRound> builder)
    {
        builder.ToTable(
            "turn_game_rounds",
            table => table.HasCheckConstraint(
                "ck_turn_game_rounds_number_positive",
                "number > 0"));

        builder.HasKey(round => round.Id);

        builder.Property(round => round.Id)
            .ValueGeneratedNever();

        builder.HasIndex(round => new { round.TurnGameId, round.Number })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(round => round.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(round => round.Status)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();

        builder.Property(round => round.StartedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(round => round.FinishedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasMany(round => round.Moves)
            .WithOne()
            .HasForeignKey(move => move.TurnGameRoundId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(round => round.Moves)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(round => round.IsDraw);
        builder.Ignore(round => round.DomainEvents);
    }
}
