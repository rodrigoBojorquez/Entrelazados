namespace DatePlanner.Domain.Dates;

public interface IDateRepository
{
    Task<Date?> GetDateByIdAsync(Guid dateId);
    
    void Insert(Date date);
}