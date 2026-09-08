namespace DatePlanner.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    
    Task<User?> GetUserByIdAsync(Guid userId);

    Task<User?> GetUserByPartnerCodeAsync(string partnerCode);
    
    void Insert(User user);
}
