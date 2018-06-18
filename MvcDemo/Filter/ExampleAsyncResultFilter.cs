using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleAsyncResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleAsyncResultFilter)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleAsyncResultFilter)}");
        }
    }
}
