using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.RevokeRefreshToken;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<Result>;
