using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples.Events;

public sealed class RelationshipBeganDomainEvent : DomainEvent
{
    internal RelationshipBeganDomainEvent(
        Guid coupleId,
        Guid partnerOneId,
        Guid partnerTwoId)
    {
        CoupleId = coupleId;
        PartnerOneId = partnerOneId;
        PartnerTwoId = partnerTwoId;
    }

    public Guid CoupleId { get; }

    public Guid PartnerOneId { get; }

    public Guid PartnerTwoId { get; }
}
