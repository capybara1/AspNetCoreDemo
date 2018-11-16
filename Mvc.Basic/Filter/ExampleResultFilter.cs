using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleResultFilter)}");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            Debug.WriteLine($"In post action code of {nameof(ExampleResultFilter)}");
        }
    }
}
