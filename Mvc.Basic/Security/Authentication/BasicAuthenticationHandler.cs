using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspNetCoreDemo.Mvc.Basic.Contracts;

namespace AspNetCoreDemo.Mvc.Basic.Security.Authentication
{
    // Example is based on https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2

    /// <summary>
    /// Provides a handler for the 'Basic' auth4entication scheme.
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly IUserInfoService _userInfoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="encoder">The URL encoder.</param>
        /// <param name="clock">The system clock.</param>
        /// <param name="userInfoService">The user-info service.</param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserInfoService userInfoService)
            : base(options, loggerFactory, encoder, clock)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(nameof(BasicAuthenticationHandler));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var header = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(header))
            {
                _logger.LogDebug("No Authentication header set");
                return AuthenticateResult.NoResult();
            }
            if (!AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                _logger.LogDebug("Authentication header is invalid");
                return AuthenticateResult.NoResult();
            }
            if (!string.Equals(headerValue.Scheme, Scheme.Name, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug($"Authentication header scheme is not '{Scheme.Name}'");
                return AuthenticateResult.NoResult();
            }
            if (!TryGetCredentials(headerValue.Parameter, out var username, out var password))
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }

            var isUser = await _userInfoService.IsUserAsync(username, password);
            if (!isUser)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.AuthenticationMethod, "basic"),
                new Claim(ClaimTypes.Name, username),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        /// <inheritdoc />
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }

        private bool TryGetCredentials(string parameter, out string username, out string password)
        {
            var unencodedParameter = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
            var unencodedParameterParts = unencodedParameter.Split(':');
            if (unencodedParameterParts.Length != 2)
            {
                _logger.LogDebug("Authentication header parameter is not valid");
                username = null;
                password = null;
                return false;
            }
            username = unencodedParameterParts[0];
            password = unencodedParameterParts[1];
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogDebug("Authentication header does not contain a username");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogDebug("Authentication header does not contain a password");
                return false;
            }
            return true;
        }
    }
}
