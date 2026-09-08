using DatePlanner.Domain.Authentication;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.Authentication;

internal sealed class RefreshTokenRepository(DatePlannerDbContext dbContext)
    : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash) =>
        dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken =>
            refreshToken.TokenHash == tokenHash);

    public async Task<IReadOnlyCollection<RefreshToken>> GetActiveByFamilyIdAsync(
        Guid familyId,
        DateTime utcNow) =>
        await dbContext.RefreshTokens
            .Where(refreshToken =>
                refreshToken.FamilyId == familyId &&
                refreshToken.RevokedAtUtc == null &&
                refreshToken.ExpiresAtUtc > utcNow)
            .ToListAsync();

    public async Task<IReadOnlyCollection<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        DateTime utcNow) =>
        await dbContext.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == userId &&
                refreshToken.RevokedAtUtc == null &&
                refreshToken.ExpiresAtUtc > utcNow)
            .ToListAsync();

    public void Insert(RefreshToken refreshToken) =>
        dbContext.RefreshTokens.Add(refreshToken);
}
