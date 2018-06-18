using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleActionFilter)}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Debug.WriteLine($"In post action code of {nameof(ExampleActionFilter)}");
        }
    }
}
