using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Authentication;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid FamilyId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsActive(DateTime utcNow) =>
        RevokedAtUtc is null && ExpiresAtUtc > utcNow;

    public static Result<RefreshToken> Create(
        Guid userId,
        Guid familyId,
        string tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        if (expiresAtUtc <= createdAtUtc)
        {
            return Result.Failure<RefreshToken>(RefreshTokenErrors.InvalidExpiration);
        }

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FamilyId = familyId,
            TokenHash = tokenHash,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public Result Revoke(DateTime revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
        {
            return Result.Failure(RefreshTokenErrors.AlreadyRevoked);
        }

        RevokedAtUtc = revokedAtUtc;
        return Result.Success();
    }

    public Result Rotate(Guid replacementTokenId, DateTime revokedAtUtc)
    {
        Result result = Revoke(revokedAtUtc);

        if (result.IsFailure)
        {
            return result;
        }

        ReplacedByTokenId = replacementTokenId;
        return Result.Success();
    }
}
