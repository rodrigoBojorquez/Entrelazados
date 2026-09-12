using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.SetCollaborativeEditing;

public sealed record SetCollaborativeEditingCommand(
    Guid UserId,
    Guid DatePlanId,
    bool IsCollaborative) : IRequest<Result>;
