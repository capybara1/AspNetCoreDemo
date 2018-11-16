using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Mvc.Basic.Contracts
{
    /// <summary>
    /// Exposes members that can be used to determine whether
    /// a user is authorizes to perform a particular action.
    /// </summary>
    public interface IUserAuthorizationService
    {
        /// <summary>
        /// Determines whether the specified user is authorized
        /// to call the action, identified by its route name.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="method">The method.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <returns><c>true</c> if the user is authorized; otherwise, <c>false</c>.</returns>
        Task<bool> IsAuthorized(
            ClaimsPrincipal user,
            string method,
            string routeName);
    }
}
