using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AspNetCoreDemo.Mvc.Basic.Controllers
{
    [Route("authz")]
    [Authorize]
    public class AuthorizationExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public AuthorizationExamplesController(ILogger<AuthorizationExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet("anonymous")]
        [AllowAnonymous]
        public IActionResult AnonymousAuthenticationAllowed()
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            _logger.LogInformation($"Action: {nameof(actionName)}");

            return NoContent();
        }

        [HttpGet("")]
        public IActionResult AuthorizeWithoutSpecificPolicy()
        {
            ProvideDiagnosticInfo();

            return NoContent();
        }

        [HttpGet("specific")]
        [Authorize(AuthenticationSchemes = "DemoScheme")]
        public IActionResult AuthorizeWithSpecificScheme()
        {
            ProvideDiagnosticInfo();

            return NoContent();
        }

        [HttpGet("roles/any")]
        [Authorize(Roles = "Contributor,Admin")]
        public IActionResult AuthorizeWithSetOfRoleAlternatives()
        {
            ProvideDiagnosticInfo();

            return NoContent();
        }

        [HttpGet("roles/all")]
        [Authorize(Roles = "Contributor")]
        [Authorize(Roles = "Admin")]
        public IActionResult AuthorizeWithSetOfRequiredRoles()
        {
            ProvideDiagnosticInfo();

            return NoContent();
        }

        [HttpGet("policy")]
        [Authorize(Policy = "DemoPolicy")]
        public IActionResult AuthorizeWithSpecificPolicy()
        {
            ProvideDiagnosticInfo();

            return NoContent();
        }

        private void ProvideDiagnosticInfo()
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            _logger.LogInformation($"Action: {nameof(actionName)}");

            var principal = HttpContext.User;
            var primaryIdentity = principal.Identity;
            if (primaryIdentity != null)
            {
                _logger.LogInformation($"Is authenticated: {primaryIdentity.IsAuthenticated}");
                if (primaryIdentity is ClaimsIdentity claimsIdentity)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        _logger.LogInformation($"Value of claim of type '{claim.Type}' issued by {claim.Issuer}: {claim.Value}");
                    }
                }
            }
        }
    }

}
