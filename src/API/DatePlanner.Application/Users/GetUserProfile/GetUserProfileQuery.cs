using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Users.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse>>;