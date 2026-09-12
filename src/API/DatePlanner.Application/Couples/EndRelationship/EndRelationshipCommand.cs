using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Couples.EndRelationship;

public sealed record EndRelationshipCommand(Guid UserId) : IRequest<Result>;
