using System.Security.Claims;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

public static class CurrentUserHelper
{
    public static (Guid? userId, EmployeeRole? role) GetCurrentUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("id");
        var roleClaim = user.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null || roleClaim == null || 
            !Guid.TryParse(userIdClaim.Value, out var userId) ||
            !Enum.TryParse<EmployeeRole>(roleClaim.Value, out var role))
        {
            return (null, null);
        }

        return (userId, role);
    }
}