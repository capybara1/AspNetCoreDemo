using AspNetCoreDemo.MvcDemo.Filter;
using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    public class TestController : ControllerBase
    {
        private readonly ILogger _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public void NoResultAction()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(NoResultAction)}");
        }

        public ExampleModel ModelResultAction()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(ModelResultAction)}");
            return new ExampleModel
            {
                Text = "my example",
            };
        }

        public IActionResult GenericResultAction()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(GenericResultAction)}");
            return NoContent();
        }

        public ActionResult<ExampleModel> GenericResultActionOrModelResult()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(GenericResultActionOrModelResult)}");
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            return new ExampleModel
            {
                Text = "my example",
            };
        }

        public IActionResult FailingAction()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(FailingAction)}");
            throw new System.Exception("Unexpected error");
        }

        [ServiceFilter(typeof(ExampleAsyncActionFilterWithDependencyInjection))]
        public IActionResult ActionWithDependencyInjectionFilter()
        {
            _logger.LogInformation($"In {nameof(TestController)}.{nameof(ActionWithDependencyInjectionFilter)}");
            return Ok("Hello, Other World!");
        }
    }
}
