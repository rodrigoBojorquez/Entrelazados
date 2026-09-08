namespace DatePlanner.Domain.Authentication;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

    Task<IReadOnlyCollection<RefreshToken>> GetActiveByFamilyIdAsync(
        Guid familyId,
        DateTime utcNow);

    Task<IReadOnlyCollection<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        DateTime utcNow);

    void Insert(RefreshToken refreshToken);
}
