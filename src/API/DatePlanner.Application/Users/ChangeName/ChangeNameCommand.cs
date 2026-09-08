using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Users.ChangeName;

public sealed record ChangeNameCommand(
    Guid UserId,
    string FirstName,
    string LastName) : IRequest<Result>;