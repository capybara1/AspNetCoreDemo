using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreDemo.MvcDemo.Security.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AspNetCoreDemo.MvcDemo.Contracts;

namespace AspNetCoreDemo.MvcDemo.Security.Authorization.Handler
{
    /// <summary>
    /// Evaluates the <see cref="IsAdminRequirement"/>.
    /// </summary>
    public class IsAdminHandler : AuthorizationHandler<IsAdminRequirement>
    {
        private readonly ILogger _logger;
        private readonly IUserInfoService _userInfoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAdminHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userInfoService">The user-info service service.</param>
        public IsAdminHandler(
            ILogger<IsAdminHandler> logger,
            IUserInfoService userInfoService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        /// <inheritdoc />
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            var isAdmin = await EvaluateAsync(context);
            if (isAdmin)
            {
                context.Succeed(requirement);
            }
        }

        /// <summary>
        /// Evaluates the given context.
        /// </summary>
        /// <param name="context">The authorizaion handler context.</param>
        /// <returns><c>true</c> if the user is authorized; otherwise, <c>false</c></returns>
        public async Task<bool> EvaluateAsync(AuthorizationHandlerContext context)
        {
            var user = context.User;
            return await _userInfoService.IsAdminAsync(user.Identity.Name);
        }
    }
}
