using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            Debug.WriteLine($"In {nameof(ExampleAsyncExceptionFilter)}");

            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
