using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.Authentication;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Id)
            .ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(refreshToken => refreshToken.UserId);
        builder.HasIndex(refreshToken => refreshToken.FamilyId);
        builder.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();

        builder.Property(refreshToken => refreshToken.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(refreshToken => refreshToken.ExpiresAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(refreshToken => refreshToken.RevokedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Ignore(refreshToken => refreshToken.DomainEvents);
    }
}
