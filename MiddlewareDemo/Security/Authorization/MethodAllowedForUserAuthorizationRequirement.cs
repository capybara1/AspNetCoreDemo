using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreDemo.MiddlewareDemo.Security.Authorization
{
    public class MethodAllowedForUserAuthorizationRequirement : IAuthorizationRequirement
    { }
}
