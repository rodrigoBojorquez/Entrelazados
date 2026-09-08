using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<AuthenticationResponse>>;
