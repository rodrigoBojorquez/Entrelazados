using DatePlanner.Domain.Couples;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.Couples;

internal sealed class CoupleRepository(DatePlannerDbContext dbContext) : ICoupleRepository
{
    public async Task<Couple?> GetCoupleByIdAsync(Guid coupleId) =>
        dbContext.Couples.Local.FirstOrDefault(couple => couple.Id == coupleId)
        ?? await dbContext.Couples.FirstOrDefaultAsync(couple => couple.Id == coupleId);

    public Task<Couple?> GetCoupleByUserIdAsync(Guid userId) =>
        dbContext.Couples.FirstOrDefaultAsync(couple =>
            couple.IsActive &&
            (couple.PartnerOneId == userId || couple.PartnerTwoId == userId));

    public void Insert(Couple couple) => dbContext.Couples.Add(couple);
}
