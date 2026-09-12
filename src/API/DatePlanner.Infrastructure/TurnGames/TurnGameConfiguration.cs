using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.TurnGames;

internal sealed class TurnGameConfiguration : IEntityTypeConfiguration<TurnGame>
{
    public void Configure(EntityTypeBuilder<TurnGame> builder)
    {
        builder.ToTable(
            "turn_games",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_turn_games_different_players",
                    "player_one_id <> player_two_id");
                table.HasCheckConstraint(
                    "ck_turn_games_valid_winner",
                    "winner_id IS NULL OR winner_id = player_one_id OR winner_id = player_two_id");
            });

        builder.HasKey(game => game.Id);

        builder.Property(game => game.Id)
            .ValueGeneratedNever();

        builder.HasOne<Couple>()
            .WithMany()
            .HasForeignKey(game => game.CoupleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(game => game.PlayerOneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(game => game.PlayerTwoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(game => game.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(game => game.CoupleId);
        builder.HasIndex(game => game.CoupleId)
            .IsUnique()
            .HasDatabaseName("ix_turn_games_active_planning_decision")
            .HasFilter("purpose = 'PlanningDecision' AND status <> 'Finished'");
        builder.HasIndex(game => game.PlayerOneId);
        builder.HasIndex(game => game.PlayerTwoId);

        builder.Property(game => game.GameType)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(game => game.Purpose)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(TurnGamePurpose.Casual)
            .IsRequired();

        builder.Property(game => game.Status)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(game => game.CreatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(game => game.StartedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(game => game.FinishedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasMany(game => game.Rounds)
            .WithOne()
            .HasForeignKey(round => round.TurnGameId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(game => game.Rounds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(game => game.DomainEvents);
    }
}
