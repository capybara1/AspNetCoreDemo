using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleActionFilterAttribute)}.{nameof(OnActionExecutionAsync)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleActionFilterAttribute)}.{nameof(OnActionExecutionAsync)}");
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleActionFilterAttribute)}.{nameof(OnResultExecutionAsync)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleActionFilterAttribute)}.{nameof(OnResultExecutionAsync)}");
        }
    }


}
