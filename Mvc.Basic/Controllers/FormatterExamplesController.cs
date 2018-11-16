using AspNetCoreDemo.Mvc.Basic.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreDemo.Mvc.Basic.Controllers
{
    [Route("formatter")]
    public class FormatterExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public FormatterExamplesController(ILogger<FormatterExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpPut]
        public IActionResult Put([FromBody]ExampleModel example)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.SelectMany(kvp => kvp.Value.Errors.Select(err => new { kvp.Key, Message = err.ErrorMessage })))
                {
                    _logger.LogInformation($"Error at {error.Key}: {error.Message}");
                }
                return BadRequest();
            }

            return NoContent();
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = new ExampleModel
            {
                Priority = 2,
                Text = "example text",
            };

            return Ok(model);
        }
    }
}
