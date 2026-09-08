using DatePlanner.Domain.Users;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.Users;

internal sealed class UserRepository(DatePlannerDbContext dbContext) : IUserRepository
{
    public Task<User?> GetUserByEmailAsync(string email) =>
        dbContext.Users
            .Include(user => user.DatingPreferences)
            .FirstOrDefaultAsync(user => user.Email == email);

    public Task<User?> GetUserByIdAsync(Guid userId) =>
        dbContext.Users
            .Include(user => user.DatingPreferences)
            .FirstOrDefaultAsync(user => user.Id == userId);

    public Task<User?> GetUserByPartnerCodeAsync(string partnerCode) =>
        dbContext.Users
            .Include(user => user.DatingPreferences)
            .FirstOrDefaultAsync(user => user.PartnerCode == partnerCode);

    public void Insert(User user) => dbContext.Users.Add(user);
}
