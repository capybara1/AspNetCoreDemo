using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("ApiController")]
    [ApiController]
    [Produces("application/json")] // Required to perform output formatting
    public class ApiControllerExamplesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<string> TestAction(int param)
        {
            if (param > 10)
            {
                return BadRequest();
            }
            return "Hello, World!";
        }
    }
}