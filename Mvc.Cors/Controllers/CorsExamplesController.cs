using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [EnableCors("DemoCorsPolicy")]
    public class DemoController : ControllerBase
    {
        [HttpGet("/enabled")]
        public string AccessControlledByCorsPolicy()
        {
            return "Hello, World!";
        }

        [HttpGet("/disabled")]
        [DisableCors]
        public string AccessNotControlledByCorsPolicy()
        {
            return "Hello, World!";
        }
    }
}
