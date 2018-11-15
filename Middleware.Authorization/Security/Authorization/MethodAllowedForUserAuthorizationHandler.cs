using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Middleware.Authorization.Security.Authorization
{
    public class MethodAllowedForUserAuthorizationHandler
        : AuthorizationHandler<MethodAllowedForUserAuthorizationRequirement>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;

        public MethodAllowedForUserAuthorizationHandler(IUserAuthorizationService userAuthorizationService)
        {
            _userAuthorizationService = userAuthorizationService ?? throw new System.ArgumentNullException(nameof(userAuthorizationService));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MethodAllowedForUserAuthorizationRequirement requirement)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            if (requirement == null) throw new System.ArgumentNullException(nameof(requirement));

            var user = context.User;

            var isMethodAllowedForUser = _userAuthorizationService.IsMethodAllowedForUser(user);
            if (isMethodAllowedForUser)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
