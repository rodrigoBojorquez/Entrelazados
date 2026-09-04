using DatePlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatePlanner.Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property<string>("PasswordHash")
            .HasColumnName("password_hash")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(user => user.PartnerCode)
            .HasMaxLength(12)
            .IsRequired();

        builder.HasIndex(user => user.PartnerCode)
            .IsUnique();

        builder.Ignore(user => user.FullName);
        builder.Ignore(user => user.DomainEvents);
    }
}
