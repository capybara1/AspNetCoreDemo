using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreDemo.MvcDemo.Security.Authentication
{
    // Example is based on https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2

    /// <summary>
    /// Options for the basic authentication scheme.
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        public string Realm { get; set; }
    }
}