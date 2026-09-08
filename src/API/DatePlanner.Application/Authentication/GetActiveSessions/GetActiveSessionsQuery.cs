using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.GetActiveSessions;

public sealed record GetActiveSessionsQuery(Guid UserId)
    : IRequest<Result<IReadOnlyCollection<ActiveSessionResponse>>>;

public sealed record ActiveSessionResponse(
    Guid Id,
    Guid FamilyId,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc);
