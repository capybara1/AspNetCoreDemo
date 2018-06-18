using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("doc")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiExplorerExamplesController))]
    public class ApiExplorerExamplesController : ControllerBase
    {
        private readonly ILogger<ApiExplorerExamplesController> _logger;
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
    }

}
