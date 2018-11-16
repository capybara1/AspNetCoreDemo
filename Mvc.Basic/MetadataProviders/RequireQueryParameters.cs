using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCoreDemo.Mvc.Basic.MetadataProviders
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
