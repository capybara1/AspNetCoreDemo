using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("versioning")]
    //[Route("v{version:apiVersion}/versioning")] // Only required for UrlSegmentApiVersionReader
    [ApiVersion("1.0")]
    public class FirstVersioningExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public FirstVersioningExamplesController(ILogger<AttributeRoutingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet("")]
        public IActionResult VersionedAction()
        {
            _logger.LogInformation($"In {nameof(FirstVersioningExamplesController)}");
            return Ok("Hello, World!");
        }
    }
}
