using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    public class ConventionBasedRoutingExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ConventionBasedRoutingExamplesController(ILogger<ConventionBasedRoutingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public IActionResult Get(int? id)
        {
            _logger.LogInformation($"In {nameof(ConventionBasedRoutingExamplesController)}");
            _logger.LogInformation($"Povided id is {id}");
            return Ok("Hello, World!");
        }
    }
}
