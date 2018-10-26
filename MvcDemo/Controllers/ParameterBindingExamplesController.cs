using AspNetCoreDemo.MvcDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.MvcDemo.Controllers
{
    [Route("pb")]
    public class ParameterBindingExamplesController : ControllerBase
    {
        private readonly ILogger _logger;

        public ParameterBindingExamplesController(ILogger<FormatterExamplesController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpPost("simple_parameter/plain/{id}")]
        public void SetExampleWithPlainInlineParams(string id, int priority, string text)
        {
            _logger.LogInformation($"Id parameter value: '{id}'");
            _logger.LogInformation($"Priority parameter value: '{priority}'");
            _logger.LogInformation($"Text parameter value: '{text}'");
        }

        [HttpPost("simple_parameter/annotated/{id}")]
        public void SetExampleWithAnnotatedInlineParams(
            [FromRoute]string id,
            [FromForm(Name ="priority")]int priorityFromForm,
            [FromQuery(Name = "priority")]int priorityFromQuery,
            [FromQuery]string text)
        {
            _logger.LogInformation($"Id parameter value: '{id}'");
            _logger.LogInformation($"Priority parameter value from form: '{priorityFromForm}'");
            _logger.LogInformation($"Priority parameter value from query: '{priorityFromQuery}'");
            _logger.LogInformation($"Text parameter value: '{text}'");
        }

        [HttpGet("array_parameter")]
        public void GetExampleWithArrayParameter(string[] param)
        {
            foreach (var paramValue in param)
            {
                _logger.LogInformation($"Parameter value: '{paramValue}'");
            }
        }

        [HttpPost("complex_parameter/qualified/{example.id}")]
        public void GetExampleWithBindingToComplexObject(ComplexBindingExampleModel example)
        {
            _logger.LogInformation($"Id parameter value: '{example?.Id}'");
            _logger.LogInformation($"Priority parameter value: '{example?.Priority}'");
            _logger.LogInformation($"Text parameter value: '{example?.Text}'");
        }

        [HttpPost("complex_parameter/unqualified/{id}")]
        public void GetExampleWithBindingToComplexObjectFromRouteParameter(ComplexBindingExampleModel example)
        {
            _logger.LogInformation($"Id parameter value: '{example?.Id}'");
            _logger.LogInformation($"Priority parameter value: '{example?.Priority}'");
            _logger.LogInformation($"Text parameter value: '{example?.Text}'");
        }

        [HttpGet("custom_binder")]
        public void SetExampleWithCustomBinder(SortExpression filter)
        {
            foreach (var item in filter)
            {
                _logger.LogInformation($"Filter parameter direction: '{item.Direction}'");
                _logger.LogInformation($"Filter parameter field name: '{item.FieldName}'");
            }
        }
    }
}
