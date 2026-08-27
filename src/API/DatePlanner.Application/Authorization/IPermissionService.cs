using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Application.Authorization;

public interface IPermissionService
{
    Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId);
}
