using System.Threading.Tasks;
using AspNetCoreDemo.MvcDemo.Contracts;
using AspNetCoreDemo.MvcDemo.Security.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreDemo.MvcDemo.Security.Authorization.Handler
{
    /// <summary>
    /// Evaluates the <see cref="MethodAllowedForUserRequirement"/>.
    /// </summary>
    public class UserIsAuthorizedHandler
        : AuthorizationHandler<UserIsAuthorizedRequirement>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAdminHandler"/> class.
        /// </summary>
        /// <param name="userAuthorizationService">The user-authorization service.</param>
        public UserIsAuthorizedHandler(IUserAuthorizationService userAuthorizationService)
        {
            _userAuthorizationService = userAuthorizationService ?? throw new System.ArgumentNullException(nameof(userAuthorizationService));
        }

        /// <inheritdoc />
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserIsAuthorizedRequirement requirement)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            if (requirement == null) throw new System.ArgumentNullException(nameof(requirement));

            if (context.Resource is AuthorizationFilterContext filterContext)
            {
                var user = context.User;
                var method = filterContext.HttpContext.Request.Method;
                var routeName = filterContext.ActionDescriptor.AttributeRouteInfo?.Name;

                var isUserAuthorized = await _userAuthorizationService.IsAuthorized(
                    user,
                    method,
                    routeName);
                if (isUserAuthorized)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
