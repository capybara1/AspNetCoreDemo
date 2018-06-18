using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("explicit")]
    public class ExplicitRoutingExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ExplicitRoutingExamplesController(ILogger<ExplicitRoutingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [Route("test")]
        public IActionResult Action()
        {
            _logger.LogInformation($"In {nameof(ExplicitRoutingExamplesController)}");
            return Ok("Hello, Explicit World!");
        }
    }
}
