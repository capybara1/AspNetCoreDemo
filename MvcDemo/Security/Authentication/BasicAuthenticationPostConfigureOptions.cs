using System;
using Microsoft.Extensions.Options;

namespace AspNetCoreDemo.MvcDemo.Security.Authentication
{
    // Example is based on https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2

    /// <summary>
    /// Post configuration options for the basic authentication scheme.
    /// </summary>
    public class BasicAuthenticationPostConfigureOptions : IPostConfigureOptions<BasicAuthenticationOptions>
    {
        /// <inheritdoc />
        public void PostConfigure(string name, BasicAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}