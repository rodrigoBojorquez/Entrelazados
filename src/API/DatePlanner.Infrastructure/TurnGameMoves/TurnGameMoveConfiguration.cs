using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.TurnGameMoves;

internal sealed class TurnGameMoveConfiguration : IEntityTypeConfiguration<TurnGameMove>
{
    public void Configure(EntityTypeBuilder<TurnGameMove> builder)
    {
        builder.ToTable(
            "turn_game_moves",
            table => table.HasCheckConstraint(
                "ck_turn_game_moves_sequence_positive",
                "sequence > 0"));

        builder.HasKey(move => move.Id);

        builder.Property(move => move.Id)
            .ValueGeneratedNever();

        builder.HasIndex(move => new { move.TurnGameRoundId, move.Sequence })
            .IsUnique();

        builder.HasIndex(move => new { move.TurnGameRoundId, move.PlayerId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(move => move.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(move => move.Code)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(move => move.PlayedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Ignore(move => move.DomainEvents);
    }
}
