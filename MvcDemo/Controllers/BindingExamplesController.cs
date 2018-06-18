using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("be")]
    public class BindingExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public BindingExamplesController(ILogger<BindingExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpPut("test1/{name}")]
        public IActionResult SetExampleWithInputModel(SetExampleInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.SelectMany(kvp => kvp.Value.Errors.Select(err => new { kvp.Key, Message = err.ErrorMessage })))
                {
                    _logger.LogInformation($"Error at {error.Key}: {error.Message}");
                }
                return BadRequest();
            }

            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(SetExampleWithInputModel)}");
            return NoContent();
        }

        [HttpPut("test2/{name}")]
        public IActionResult SetExampleWithInlineParams(string name, [FromBody]ExampleModel example)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.SelectMany(kvp => kvp.Value.Errors.Select(err => new { kvp.Key, Message = err.ErrorMessage })))
                {
                    _logger.LogInformation($"Error at {error.Key}: {error.Message}");
                }
                return BadRequest();
            }

            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(SetExampleWithInlineParams)}");
            return NoContent();
        }

        [HttpGet("test3")]
        public IActionResult GetExample()
        {
            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(GetExample)}");

            var model = new ExampleModel
            {
                Priority = 2,
                Text = "example text",
            };

            return Ok(model);
        }

        [HttpGet("array_parameter")]
        public IActionResult GetExampleWithArrayParameter(string[] param)
        {
            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(GetExample)}");

            foreach (var paramValue in param)
            {
                _logger.LogInformation($"Parameter value: '{paramValue}'");
            }

            var model = new ExampleModel
            {
                Priority = 2,
                Text = "example text",
            };

            return Ok(model);
        }

        [HttpGet("complex_parameter")]
        public IActionResult GetExampleWithComplexParameter(ExampleModel example)
        {
            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(GetExample)}");

            _logger.LogInformation($"Priority parameter value: '{example?.Priority}'");
            _logger.LogInformation($"Text parameter value: '{example?.Text}'");

            var model = new ExampleModel
            {
                Priority = example.Priority,
                Text = example.Text,
            };

            return Ok(model);
        }

        [HttpPut("custom_binder")]
        public IActionResult SetExampleWithCustomBinder(SortExpression filterExpression)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.SelectMany(kvp => kvp.Value.Errors.Select(err => new { kvp.Key, Message = err.ErrorMessage })))
                {
                    _logger.LogInformation($"Error at {error.Key}: {error.Message}");
                }
                return BadRequest();
            }

            _logger.LogInformation($"In {nameof(BindingExamplesController)}.{nameof(SetExampleWithInlineParams)}");
            return NoContent();
        }
    }
}
