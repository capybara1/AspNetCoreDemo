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

        [HttpGet("/absolute_route")]
        public string ActionWithAbsoluteRoute()
        {
            return nameof(ActionWithAbsoluteRoute);
        }

        [HttpGet]
        [HttpPut]
        [HttpDelete]
        [Route("test1")]
        public string ActionWithMultipleMethods()
        {
            return "Hello, Explicit World!";
        }

        [HttpGet]
        [Route("test2/first")]
        [Route("test2/second")]
        public string ActionWithMultipleRoutes()
        {
            return "Hello, Explicit World!";
        }

        [HttpGet("test3")]
        public string FirstActionWithConflictingUnorderedRoutes()
        {
            return nameof(FirstActionWithConflictingUnorderedRoutes);
        }

        [HttpGet("test3/morespecific")] // Executes before less specific routes like "test1"
        public string SecondActionWithConflictingUnorderedRoutes()
        {
            return nameof(SecondActionWithConflictingUnorderedRoutes);
        }

        [HttpGet]
        [Route("test4", Order = 1)]
        public string FirstActionWithConflictingOrderedRoutes()
        {
            return nameof(FirstActionWithConflictingOrderedRoutes);
        }

        [HttpGet]
        [Route("test4/morespecific", Order = 2)]
        public string SecondActionWithConflictingOrderedRoutes()
        {
            return nameof(SecondActionWithConflictingOrderedRoutes);
        }

        [HttpGet("test5/{param:int}")]
        public string FirstActionWithConstrainedParameterInRoute(int param)
        {
            return nameof(FirstActionWithConstrainedParameterInRoute);
        }

        [HttpGet("test5/{param:alpha}")]
        public string SecondActionWithConstrainedParameterInRoute(string param)
        {
            return nameof(SecondActionWithConstrainedParameterInRoute);
        }

        [HttpGet("test6/{*remainder}")]
        public string ActionWithCatchAll(string remainder)
        {
            _logger.LogInformation($"{nameof(remainder)} = \"{remainder}\"");
            return nameof(ActionWithCatchAll);
        }

        [HttpGet("test7")]
        public string FirstActionWithConflictingNamedRoute([Required]string param)
        {
            return nameof(FirstActionWithConflictingNamedRoute);
        }

        [HttpGet("test7")]
        public string SecondActionWithConflictingNamedRoute()
        {
            return nameof(SecondActionWithConflictingNamedRoute);
        }
    }
}
