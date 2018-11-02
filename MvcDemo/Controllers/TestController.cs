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
        { }

        public ExampleModel ModelResultAction()
        {
            return new ExampleModel
            {
                Text = "my example",
            };
        }

        public IActionResult GenericResultAction()
        {
            return NoContent();
        }

        public ActionResult<ExampleModel> GenericResultActionOrModelResult()
        {
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
            throw new System.Exception("Unexpected error");
        }

        [ServiceFilter(typeof(ExampleAsyncActionFilterWithDependencyInjection))]
        public IActionResult ActionWithDependencyInjectionFilter()
        {
            return Ok("Hello, Other World!");
        }
    }
}
