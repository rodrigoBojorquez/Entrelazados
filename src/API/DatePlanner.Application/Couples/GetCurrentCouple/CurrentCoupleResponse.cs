namespace DatePlanner.Application.Couples.GetCurrentCouple;

public sealed record CurrentCoupleResponse(
    Guid Id,
    Guid PartnerOneId,
    Guid PartnerTwoId,
    DateTime BeginRelationshipAt,
    bool IsActive);