using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreDemo.Mvc.Basic.Security.Authorization.Requirements
{
    /// <summary>
    /// Represents the authorization requirement that the user is
    /// authorized to access the resource.
    /// </summary>
    public class UserIsAuthorizedRequirement : IAuthorizationRequirement
    { }
}
