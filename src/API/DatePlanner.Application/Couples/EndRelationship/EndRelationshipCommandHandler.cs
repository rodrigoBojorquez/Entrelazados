using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using MediatR;

namespace DatePlanner.Application.Couples.EndRelationship;

public sealed class EndRelationshipCommandHandler(
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<EndRelationshipCommand, Result>
{
    public async Task<Result> Handle(
        EndRelationshipCommand request,
        CancellationToken cancellationToken)
    {
        Couple? couple = await coupleRepository.GetCoupleByUserIdAsync(request.UserId);

        if (couple is null)
        {
            return Result.Failure(CoupleErrors.NotFound(request.UserId));
        }

        Result endResult = couple.EndRelationship();

        if (endResult.IsFailure)
        {
            return endResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
