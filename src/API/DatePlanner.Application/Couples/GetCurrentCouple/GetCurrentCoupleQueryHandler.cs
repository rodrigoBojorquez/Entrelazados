using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentCouple;

public sealed class GetCurrentCoupleQueryHandler(ICoupleRepository coupleRepository)
    : IRequestHandler<GetCurrentCoupleQuery, Result<CurrentCoupleResponse>>
{
    public async Task<Result<CurrentCoupleResponse>> Handle(
        GetCurrentCoupleQuery request,
        CancellationToken cancellationToken)
    {
        Couple? couple = await coupleRepository.GetCoupleByUserIdAsync(request.UserId);

        if (couple is null)
        {
            return Result.Failure<CurrentCoupleResponse>(CoupleErrors.NotFound(request.UserId));
        }

        return new CurrentCoupleResponse(
            couple.Id,
            couple.PartnerOneId,
            couple.PartnerTwoId,
            couple.BeginRelationshipAt,
            couple.IsActive);
    }
}
