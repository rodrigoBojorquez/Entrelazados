using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples;

public class Couple : Entity
{
    private Couple()
    {
    }

    public static Couple Create()
    {
        var couple = new Couple
        {
            Id = Guid.NewGuid()
        };

        return couple;
    }
    
    public Guid Id { get; private set; }
    
    public HashSet<Guid> CoupleIds { get; private set; }
    
    public DateTime BeginRelationshipAt { get; private set; }
    
    public bool IsActive { get; private set; }
}