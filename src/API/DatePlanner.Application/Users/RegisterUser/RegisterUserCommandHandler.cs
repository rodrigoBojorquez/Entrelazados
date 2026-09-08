using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        string email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.GetUserByEmailAsync(email) is not null)
        {
            return Result.Failure<Guid>(UserErrors.EmailAlreadyExists(email));
        }

        string passwordHash = passwordHasher.Hash(request.Password);
        User user = User.Create(
            email,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            passwordHash);

        userRepository.Insert(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
