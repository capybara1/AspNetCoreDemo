using System;
using System.Collections.Generic;
using AspNetCoreDemo.Mvc.Basic.Security.Authentication;
using AspNetCoreDemo.Mvc.Basic.Security.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Collections.ObjectModel;

namespace AspNetCoreDemo.Mvc.Basic.Security.Authorization
{
    /// <summary>
    /// The authorization policies.
    /// </summary>
    internal static class Policies
    {
        /// <summary>
        /// Initializes static members of the <see cref="Policies"/> class.
        /// </summary>
        static Policies()
        {
            AdministerResources = CreatePolicy(policy =>
            {
                policy.AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new UserIsAuthorizedRequirement());
            });
            ContributeResources = CreatePolicy(policy =>
            {
                policy.AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });

            All = new Dictionary<string, AuthorizationPolicy>
            {
                [nameof(AdministerResources)] = AdministerResources,
                [nameof(ContributeResources)] = ContributeResources,
            };
        }

        /// <summary>
        /// A policy that allows administration of resources.
        /// </summary>
        public static AuthorizationPolicy AdministerResources { get; }
        
        /// <summary>
        /// A policy that allows contribution of resources.
        /// </summary>
        public static AuthorizationPolicy ContributeResources { get; }

        /// <summary>
        /// Gets all known policies.
        /// </summary>
        public static IReadOnlyDictionary<string, AuthorizationPolicy> All { get; }

        private static AuthorizationPolicy CreatePolicy(Action<AuthorizationPolicyBuilder> buildAction)
        {
            var builder = new AuthorizationPolicyBuilder();
            buildAction(builder);
            var result = builder.Build();
            return result;
        }
    }
}
