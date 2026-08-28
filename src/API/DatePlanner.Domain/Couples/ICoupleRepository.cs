namespace DatePlanner.Domain.Couples;

public interface ICoupleRepository
{
    Task<Couple?> GetCoupleByIdAsync(Guid coupleId);
    
    Task<Couple?> GetCoupleByUserIdAsync(Guid userId);
    
    void Insert(Couple couple);
}