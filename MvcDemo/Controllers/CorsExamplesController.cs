using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("cors")]
    [EnableCors("DemoCorsPolicy")]
    public class CorsExamplesController : ControllerBase
    {
        [HttpGet("")]
        public string AccessControlledByCorsPolicy()
        {
            return "Hello, World!";
        }

        [HttpGet("disabled")]
        [DisableCors]
        public string AccessNotControlledByCorsPolicy()
        {
            return "Hello, World!";
        }
    }
}
