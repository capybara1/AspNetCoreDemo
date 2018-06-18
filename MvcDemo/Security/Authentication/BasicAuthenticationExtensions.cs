using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCoreDemo.MvcDemo.Security.Authentication
{
    // Example is based on https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2

    /// <summary>
    /// Extension methods to set up authentication.
    /// </summary>
    public static class BasicAuthenticationExtensions
    {
        /// <summary>
        /// Adds basic authentication.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder)
        {
            return AddBasicAuthentication(
                builder,
                BasicAuthenticationDefaults.AuthenticationScheme,
                opt =>
                {
                    opt.Realm = BasicAuthenticationDefaults.Realm;
                });
        }

        /// <summary>
        /// Adds basic authentication.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddBasicAuthentication(
                builder,
                authenticationScheme,
                opt =>
                {
                    opt.Realm = BasicAuthenticationDefaults.Realm;
                });
        }

        /// <summary>
        /// Adds basic authentication.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configureOptions">A callback to configure the options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions)
        {
            return AddBasicAuthentication(
                builder,
                BasicAuthenticationDefaults.AuthenticationScheme,
                configureOptions);
        }

        /// <summary>
        /// Adds basic authentication.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A callback to configure the options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (authenticationScheme == null) throw new ArgumentNullException(nameof(authenticationScheme));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            builder.Services.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();

            builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                authenticationScheme,
                configureOptions);

            return builder;
        }
    }
}