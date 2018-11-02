using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("explicit")]
    public class AttributeRoutingExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public AttributeRoutingExamplesController(ILogger<AttributeRoutingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [Route("test1")]
        public IActionResult ActionWithMultipleMethods()
        {
            return Ok("Hello, Explicit World!");
        }

        [HttpGet]
        [Route("test2/first")]
        [Route("test2/second")]
        public IActionResult ActionWithMultipleRoutes()
        {
            return Ok("Hello, Explicit World!");
        }

        [HttpGet("test3")]
        public IActionResult FirstActionWithConflictingUnorderedRoutes()
        {
            return Ok(nameof(FirstActionWithConflictingUnorderedRoutes));
        }

        [HttpGet("test3/morespecific")] // Executes before less specific routes like "test1"
        public IActionResult SecondActionWithConflictingUnorderedRoutes()
        {
            return Ok(nameof(SecondActionWithConflictingUnorderedRoutes));
        }

        [HttpGet]
        [Route("test4", Order = 1)]
        public IActionResult FirstActionWithConflictingOrderedRoutes()
        {
            return Ok(nameof(FirstActionWithConflictingOrderedRoutes));
        }

        [HttpGet]
        [Route("test4/morespecific", Order = 2)]
        public IActionResult SecondActionWithConflictingOrderedRoutes()
        {
            return Ok(nameof(SecondActionWithConflictingOrderedRoutes));
        }

        [HttpGet("test5/{param:int}")]
        public IActionResult FirstActionWithConstrainedParameterInRoute(int param)
        {
            return Ok(nameof(FirstActionWithConstrainedParameterInRoute));
        }

        [HttpGet("test5/{param:alpha}")]
        public IActionResult SecondActionWithConstrainedParameterInRoute(string param)
        {
            return Ok(nameof(SecondActionWithConstrainedParameterInRoute));
        }

        [HttpGet("test6/{*remainder}")]
        public IActionResult ActionWithCatchAll(string remainder)
        {
            _logger.LogInformation($"{nameof(remainder)} = \"{remainder}\"");
            return Ok(nameof(ActionWithCatchAll));
        }

        [HttpGet("test7")]
        public IActionResult FirstActionWithConflictingNamedRoute([Required]string param)
        {
            return Ok(nameof(FirstActionWithConflictingNamedRoute));
        }

        [HttpGet("test7")]
        public IActionResult SecondActionWithConflictingNamedRoute()
        {
            return Ok(nameof(SecondActionWithConflictingNamedRoute));
        }
    }
}
