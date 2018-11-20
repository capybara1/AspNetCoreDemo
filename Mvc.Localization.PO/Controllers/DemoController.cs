using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.Mvc.Localization.PO.Controllers
{
    [Route("localization")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IStringLocalizer _stringLocalizer;

        public DemoController(
            ILogger<DemoController> logger,
            IStringLocalizer<DemoController> stringLocalizer)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _stringLocalizer = stringLocalizer;
        }
        
        [HttpGet]
        public void Get()
        {
            var value = _stringLocalizer["example"];
            _logger.LogInformation($"Localized string: {value}");
        }
    }
}
