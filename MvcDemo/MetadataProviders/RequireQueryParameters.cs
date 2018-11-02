using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCoreDemo.MvcDemo.MetadataProviders
{
    public class RequireQueryParameters : IBindingMetadataProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.BindingMetadata.BindingSource == BindingSource.Query)
            {
                context.BindingMetadata.IsBindingRequired = true;
            }
        }
    }
}
