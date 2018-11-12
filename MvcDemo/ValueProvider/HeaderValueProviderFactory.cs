using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.ValueProvider
{
    public class HeaderValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var request = context.ActionContext.HttpContext.Request;
            var headers = request.Headers;
            if (headers.Count > 0)
            {
                context.ValueProviders.Add(new HeaderValueProvider(headers));
            }

            return Task.CompletedTask;
        }
    }
}
