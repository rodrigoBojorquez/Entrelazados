using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Couples.BeginRelationship;

public sealed class BeginRelationshipCommandHandler(
    IUserRepository userRepository,
    ICoupleRepository coupleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BeginRelationshipCommand, Result>
{
    public async Task<Result> Handle(
        BeginRelationshipCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        string partnerCode = request.PartnerCode.Trim();
        User? partner = await userRepository.GetUserByPartnerCodeAsync(partnerCode);

        if (partner is null)
        {
            return Result.Failure(UserErrors.PartnerCodeNotFound(partnerCode));
        }

        if (await coupleRepository.GetCoupleByUserIdAsync(user.Id) is not null)
        {
            return Result.Failure(CoupleErrors.UserAlreadyHasPartner(user.Id));
        }

        if (await coupleRepository.GetCoupleByUserIdAsync(partner.Id) is not null)
        {
            return Result.Failure(CoupleErrors.UserAlreadyHasPartner(partner.Id));
        }

        Result<Couple> creationResult = Couple.Create(user.Id, partner.Id);

        if (creationResult.IsFailure)
        {
            return Result.Failure(creationResult.Error);
        }

        coupleRepository.Insert(creationResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
