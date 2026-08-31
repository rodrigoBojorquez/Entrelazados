using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Couples.BeginRelationship;

public sealed record BeginRelationshipCommand(Guid UserId, string PartnerCode) : IRequest<Result>;
