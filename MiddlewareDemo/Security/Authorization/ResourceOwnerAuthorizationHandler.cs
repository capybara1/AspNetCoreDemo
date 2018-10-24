using AspNetCoreDemo.MiddlewareDemo.Contracts;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MiddlewareDemo.Security.Authorization
{
    public class ResourceOwnerAuthorizationHandler
        : AuthorizationHandler<ResourceOwnerAuthorizationRequirement, Resource>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceOwnerAuthorizationRequirement requirement,
            Resource resource)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));

            var isOwner = string.Equals(context.User.Identity?.Name,
                resource?.Owner?.ToString(),
                StringComparison.OrdinalIgnoreCase);
            if (isOwner)
            { 
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
