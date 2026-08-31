using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Couples.BeginRelationship;

public class BeginRelationshipCommandHandler : IRequestHandler<BeginRelationshipCommand, Result>
{
    public async Task<Result> Handle(BeginRelationshipCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}