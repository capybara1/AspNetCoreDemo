using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreDemo.MvcDemo.Security.Authorization.Requirements
{
    /// <summary>
    /// Represents the authorization requirement that the user is an administrator.
    /// </summary>
    public class IsAdminRequirement : IAuthorizationRequirement
    { }
}
