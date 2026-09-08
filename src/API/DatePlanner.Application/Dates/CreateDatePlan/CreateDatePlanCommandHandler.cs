using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.CreateDatePlan;

public sealed class CreateDatePlanCommandHandler(
    ICoupleRepository coupleRepository,
    IDatePlanRepository datePlanRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDatePlanCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateDatePlanCommand request,
        CancellationToken cancellationToken)
    {
        Couple? couple = await coupleRepository.GetCoupleByUserIdAsync(request.UserId);

        if (couple is null || !couple.IsActive)
        {
            return Result.Failure<Guid>(CoupleErrors.NotFound(request.UserId));
        }

        Result<DatePlan> creationResult = DatePlan.Create(couple.Id, request.UserId);

        if (creationResult.IsFailure)
        {
            return Result.Failure<Guid>(creationResult.Error);
        }

        datePlanRepository.Insert(creationResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return creationResult.Value.Id;
    }
}
