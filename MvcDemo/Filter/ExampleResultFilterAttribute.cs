using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleResultFilterAttribute)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleResultFilterAttribute)}");
        }
    }
}
