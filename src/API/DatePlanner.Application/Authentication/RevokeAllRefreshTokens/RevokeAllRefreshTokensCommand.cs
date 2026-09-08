using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.RevokeAllRefreshTokens;

public sealed record RevokeAllRefreshTokensCommand(Guid UserId) : IRequest<Result>;
