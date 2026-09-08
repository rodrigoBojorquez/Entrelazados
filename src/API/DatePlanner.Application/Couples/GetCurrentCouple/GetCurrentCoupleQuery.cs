using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentCouple;

public sealed record GetCurrentCoupleQuery(Guid UserId) : IRequest<Result<CurrentCoupleResponse>>;

internal sealed class GetCurrentCoupleQueryValidator : AbstractValidator<GetCurrentCoupleQuery>
{
    public GetCurrentCoupleQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}

public sealed record CurrentCoupleResponse(
    Guid Id,
    Guid PartnerOneId,
    Guid PartnerTwoId,
    DateTime BeginRelationshipAt,
    bool IsActive);
