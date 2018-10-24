using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("versioning")]
    //[Route("v{version:apiVersion}/versioning")] // Only required for UrlSegmentApiVersionReader
    [ApiVersion("2.0")]
    public class SecondVersioningExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public SecondVersioningExamplesController(ILogger<AttributeRoutingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet("")]
        public IActionResult VersionedAction()
        {
            _logger.LogInformation($"In {nameof(FirstVersioningExamplesController)}");
            return Ok("Hello, World 2.0!");
        }
    }
}
