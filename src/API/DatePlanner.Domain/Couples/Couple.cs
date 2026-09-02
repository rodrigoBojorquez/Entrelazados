using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples;

public class Couple : Entity
{
    private Couple()
    {
    }
    
    public Guid Id { get; private set; }
    
    public Guid PartnerOneId { get; private set; }
    
    public Guid PartnerTwoId { get; private set; }
    
    public DateTime BeginRelationshipAt { get; private set; }
    
    public bool IsActive { get; private set; }

    public static Result<Couple> Create(Guid partnerOneId, Guid partnerTwoId)
    {
        if (partnerOneId == Guid.Empty)
        {
            return Result.Failure<Couple>(CoupleErrors.PartnerIdIsRequired(nameof(partnerOneId)));
        }

        if (partnerTwoId == Guid.Empty)
        {
            return Result.Failure<Couple>(CoupleErrors.PartnerIdIsRequired(nameof(partnerTwoId)));
        }

        if (partnerOneId == partnerTwoId)
        {
            return Result.Failure<Couple>(CoupleErrors.SamePersonCannotBePartners(partnerOneId));
        }
        
        var couple = new Couple
        {
            Id = Guid.NewGuid(),
            PartnerOneId = partnerOneId,
            PartnerTwoId = partnerTwoId,
            BeginRelationshipAt = DateTime.UtcNow,
            IsActive = true,
        };

        return couple;
    }
}
