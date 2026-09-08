using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Users.ChangeName;

public sealed class ChangeNameCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeNameCommand, Result>
{
    public async Task<Result> Handle(
        ChangeNameCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        user.ChangeName(request.FirstName.Trim(), request.LastName.Trim());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
