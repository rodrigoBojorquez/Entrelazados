using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.Login;

public sealed record LoginCommand(string Email, string Password)
    : IRequest<Result<AuthenticationResponse>>;
