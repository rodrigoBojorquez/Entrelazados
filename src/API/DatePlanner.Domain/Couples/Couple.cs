using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples;

public class Couple : Entity
{
    private Couple()
    {
    }

    public static Couple Create(Guid userId1, Guid userId2)
    {
        var couple = new Couple
        {
            Id = Guid.NewGuid(),
            CoupleIds = new HashSet<Guid> { userId1, userId2 },
            BeginRelationshipAt = DateTime.UtcNow,
            IsActive = true,
        };

        return couple;
    }
    
    public Guid Id { get; private set; }
    
    public HashSet<Guid> CoupleIds { get; private set; }
    
    public DateTime BeginRelationshipAt { get; private set; }
    
    public bool IsActive { get; private set; }
}