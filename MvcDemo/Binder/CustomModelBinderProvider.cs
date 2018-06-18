using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace AspNetCoreDemo.MvcDemo.Binder
{
    public class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType) return null;

            if (context.Metadata.ModelType == typeof(SortExpression))
            {
                return new BinderTypeModelBinder(typeof(SortExpressionBinder));
            }

            return null;
        }
    }
}
