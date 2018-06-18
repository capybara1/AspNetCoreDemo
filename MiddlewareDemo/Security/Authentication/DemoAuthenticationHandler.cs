using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace AspNetCoreDemo.MiddlewareDemo.Security.Authentication
{
    public class DemoAuthenticationHandler : AuthenticationHandler<DemoAuthenticationSchemeOptions>
    {
        public DemoAuthenticationHandler(IOptionsMonitor<DemoAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.TryGetValue("Proof", out var proofHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (proofHeaderValues[0] != "Sound Proof")
            {
                return Task.FromResult(AuthenticateResult.Fail("Given proof not accepted"));
            }

            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "John Doe"),
                new Claim("given_names", "John"),
                new Claim("family_name", "Doe"),
                new Claim(ClaimTypes.Email, "john.doe@example.com"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "Contributor"),
            };
            var identity = new ClaimsIdentity(claims, "DemoAuthenticationType");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "DemoScheme";

            return base.HandleChallengeAsync(properties);
        }
    }
}
