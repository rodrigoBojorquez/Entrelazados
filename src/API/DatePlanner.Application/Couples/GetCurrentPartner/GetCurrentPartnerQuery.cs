using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentPartner;

public sealed record GetCurrentPartnerQuery(Guid UserId) : IRequest<Result<GetCurrentPartnerResponse>>;