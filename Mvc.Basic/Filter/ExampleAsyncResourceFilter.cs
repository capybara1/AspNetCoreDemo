using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleAsyncResourceFilter : IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleAsyncResourceFilter)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleAsyncResourceFilter)}");
        }
    }
}
