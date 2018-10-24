using System.Security.Claims;

namespace AspNetCoreDemo.MiddlewareDemo.Security.Authorization
{
    public interface IUserAuthorizationService
    {
        bool IsMethodAllowedForUser(ClaimsPrincipal user);
    }
}
