using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    // Attribute routing is a requirement for API-Controllers

    [Route("ApiController")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApiControllerExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ApiControllerExamplesController(ILogger<ApiControllerExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}", Name = "getApiControllerExample")]
        [ProducesResponseType(404)]
        public ActionResult<string> Get(int id)
        {
            _logger.LogInformation($"Value of parameter 'id' is '{id}'");

            if (id > 10)
            {
                return NotFound();
            }

            return "Hello, World!";
        }

        // By using complex parameters is inferred that the binding source of those is the body
        [HttpPost]
        public IActionResult Post(Model.ExampleModel model)
        {
            _logger.LogInformation($"Value of parameter 'model.Priority' is '{model?.Priority}'");
            _logger.LogInformation($"Value of parameter 'model.Text' is '{model?.Text}'");

            var url = Url.Link("getApiControllerExample", new { id = 10 });
            return Created(url , null);
        }

        // By using form-files it is inferred that the controller consumes multipart/form-data
        [HttpPost("form-postback")]
        public void FormPostback(IFormFile file)
        {
            _logger.LogInformation($"Filename is '{file.FileName}'");
        }
    }
}