using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("ApiController")]
    [ApiController]
    [Produces("application/json")]
    public class ApiControllerExamplesController : ControllerBase
    {
        [HttpGet("{id}", Name = "getApiControllerExample")]
        [ProducesResponseType(404)]
        public ActionResult<string> Get(int id)
        {
            if (id > 10)
            {
                return NotFound();
            }
            return "Hello, World!";
        }

        [HttpPost]
        public IActionResult Post(Model.ExampleModel model)
        {
            var url = Url.Link("getApiControllerExample", new { id = 10 });
            return Created(url , null);
        }
    }
}