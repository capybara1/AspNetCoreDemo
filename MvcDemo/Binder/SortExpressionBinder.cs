using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreDemo.MvcDemo.Binder
{
    public class SortExpressionBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new System.NotImplementedException();
        }
    }
}