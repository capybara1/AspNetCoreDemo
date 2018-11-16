using AspNetCoreDemo.Mvc.DataAnnotations.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreDemo.Mvc.DataAnnotations.Controllers
{
    [Route("validation")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet("annotated_model")]
        public IActionResult SetExampleWithInputModel(AnnotatedExampleModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.SelectMany(kvp => kvp.Value.Errors.Select(err => new { kvp.Key, Message = err.ErrorMessage })))
                {
                    _logger.LogInformation($"Error at {error.Key}: {error.Message}");
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
