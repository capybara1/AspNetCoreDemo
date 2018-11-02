using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace AspNetCoreDemo.MvcDemo.Convention
{
    public class UseGlobalApiPrefix : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var attributeRouteModel = controller.Selectors
                    .Where(s => s.AttributeRouteModel != null)
                    .Select(s => s.AttributeRouteModel)
                    .FirstOrDefault();

                if (attributeRouteModel != null)
                {
                    var newTemplate = "api/" + attributeRouteModel.Template.TrimStart('/');
                    attributeRouteModel.Template = newTemplate;
                }
            }
        }
    }
}
