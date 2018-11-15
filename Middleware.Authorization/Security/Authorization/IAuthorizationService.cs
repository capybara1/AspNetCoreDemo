using System.Security.Claims;

namespace AspNetCoreDemo.Middleware.Authorization.Security.Authorization
{
    public interface IUserAuthorizationService
    {
        bool IsMethodAllowedForUser(ClaimsPrincipal user);
    }
}
