using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentCouple;

public sealed record GetCurrentCoupleQuery(Guid UserId) : IRequest<Result<CurrentCoupleResponse>>;