using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password) : IRequest<Result<Guid>>;