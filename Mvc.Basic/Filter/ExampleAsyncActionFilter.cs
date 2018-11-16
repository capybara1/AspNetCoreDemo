using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleAsyncActionFilter)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleAsyncActionFilter)}");
        }
    }
}
