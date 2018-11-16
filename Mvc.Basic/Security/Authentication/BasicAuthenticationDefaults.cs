namespace AspNetCoreDemo.Mvc.Basic.Security.Authentication
{
    // Example is based on https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2

    /// <summary>
    /// Default values used for setting up basic authentication.
    /// </summary>
    public static class BasicAuthenticationDefaults
    {
        /// <summary>
        /// The authentication scheme identifier for basic authentication.
        /// </summary>
        public const string AuthenticationScheme = "Basic";

        /// <summary>
        /// The default realm for for basic authentication.
        /// </summary>
        public const string Realm = "IdentityManager";
    }
}