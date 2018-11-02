using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("validation")]
    public class InputValidationExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public InputValidationExamplesController(ILogger<FormatterExamplesController> logger)
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
        
        [HttpGet("custom_binder")]
        public IActionResult SetExampleWithCustomBinder(SortExpression filter)
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

        [HttpGet("required_query")]
        public IActionResult ActionWithSimpleQueryParameter([FromQuery]string queryParam)
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
