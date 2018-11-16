using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("doc")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiExplorerExamplesController))]
    public class ApiExplorerExamplesController : ControllerBase
    {

        private static IReadOnlyDictionary<BindingSource, string> ParameterLocationMap = new Dictionary<BindingSource, string>
        {
            { BindingSource.Form, "formData" },
            { BindingSource.Body, "body" },
            { BindingSource.Header, "header" },
            { BindingSource.Path, "path" },
            { BindingSource.Query, "query" }
        };

        private readonly ILogger _logger;
        private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;

        public ApiExplorerExamplesController(
            ILogger<ApiExplorerExamplesController> logger,
            IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _apiExplorer = apiExplorer ?? throw new System.ArgumentNullException(nameof(apiExplorer));
        }

        [HttpGet]
        public ActionResult<ExampleModel> Index()
        {
            var buffer = new StringWriter();
            foreach (var group in _apiExplorer.ApiDescriptionGroups.Items)
            {
                buffer.WriteLine($"Group '{group.GroupName}'");
                foreach (var api in group.Items)
                {
                    buffer.WriteLine($"- {api.HttpMethod} {api.RelativePath}");
                    foreach (var responseType in api.SupportedResponseTypes)
                    {
                        buffer.WriteLine($"  => Status Code {responseType.StatusCode}, Type {responseType.Type}");
                    }
                }
            }

            _logger.LogInformation(buffer.GetStringBuilder().ToString());

            return NoContent();
        }

        [HttpGet(nameof(ActionWithArguments) + "/{pathArgument}")]
        public ActionResult<ExampleModel> ActionWithArguments([BindRequired]string pathArgument, string queryArgument)
        {
            var buffer = new StringWriter();
            foreach (var group in _apiExplorer.ApiDescriptionGroups.Items)
            {
                buffer.WriteLine($"Group '{group.GroupName}'");
                foreach (var apiDescription in group.Items)
                {
                    buffer.WriteLine($"- {apiDescription.HttpMethod} {apiDescription.RelativePath}");
                    if (apiDescription.ParameterDescriptions.Count == 0)
                    {
                        buffer.WriteLine($" (no parameters)");
                    }
                    foreach (var parameterDescription in apiDescription.ParameterDescriptions)
                    {
                        var customAttributes = Enumerable.Empty<object>();
                        if (TryGetParameterInfo(parameterDescription, apiDescription, out var parameterInfo))
                            customAttributes = parameterInfo.GetCustomAttributes(true);
                        else if (TryGetPropertyInfo(parameterDescription, out var propertyInfo))
                            customAttributes = propertyInfo.GetCustomAttributes(true);

                        var isRequired = customAttributes.Any(attr =>
                            new[] { typeof(RequiredAttribute), typeof(BindRequiredAttribute) }.Contains(attr.GetType()));
                        var modifier = isRequired ? "Required" : "Optional";
                        if (!ParameterLocationMap.TryGetValue(parameterDescription.Source, out var location))
                        {
                            location = "query";
                        }
                        buffer.WriteLine($"  => {modifier} {location} parameter {parameterDescription.Name}, Type {parameterDescription.Type}");
                    }
                }
            }

            _logger.LogInformation(buffer.GetStringBuilder().ToString());

            return NoContent();
        }

        private static bool TryGetParameterInfo(
            ApiParameterDescription apiParameterDescription,
            ApiDescription apiDescription,
            out ParameterInfo parameterInfo)
        {
            var controllerParameterDescriptor = apiDescription.ActionDescriptor.Parameters
                .OfType<ControllerParameterDescriptor>()
                .FirstOrDefault(descriptor =>
                {
                    return (apiParameterDescription.Name == descriptor.BindingInfo?.BinderModelName)
                        || (apiParameterDescription.Name == descriptor.Name);
                });

            parameterInfo = controllerParameterDescriptor?.ParameterInfo;

            return (parameterInfo != null);
        }

        private static bool TryGetPropertyInfo(
            ApiParameterDescription apiParameterDescription,
            out PropertyInfo propertyInfo)
        {
            var modelMetadata = apiParameterDescription.ModelMetadata;

            propertyInfo = (modelMetadata?.ContainerType != null)
                ? modelMetadata.ContainerType.GetProperty(modelMetadata.PropertyName)
                : null;

            return (propertyInfo != null);
        }
    }
}