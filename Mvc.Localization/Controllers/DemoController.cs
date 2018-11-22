using AspNetCoreDemo.Mvc.DataAnnotations.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreDemo.Mvc.Localization.Controllers
{
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
        
        public void ActionForCommonLocalization()
        {
            var value = _stringLocalizer["example"];
            _logger.LogInformation($"Localized string: {value}");
        }
        
        public void ActionForDataAnnotationLocalization(AnnotatedExampleModel model)
        {
            if (!ModelState.IsValid)
            {
                var key = nameof(AnnotatedExampleModel.Value);
                foreach (var error in ModelState[key].Errors)
                {
                    _logger.LogInformation($"Localized validation error message: {error.ErrorMessage}");
                }
            }
        }
    }
}
