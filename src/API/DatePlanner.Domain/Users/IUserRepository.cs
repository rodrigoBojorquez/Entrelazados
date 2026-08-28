namespace DatePlanner.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    
    Task<User?> GetUserByIdAsync(Guid userId);
    
    void Insert(User user);
}