using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System;
using AspNetCoreDemo.MvcDemo.Model;
using System.Linq;
using System.Collections.Generic;

namespace AspNetCoreDemo.MvcDemo.Binder
{
    public class SortExpressionBinder : IModelBinder
    {
        public static readonly Regex ValidationPattern = new Regex(@"-?\w(?:,-?\w)*");

        private readonly ILogger _logger;

        public SortExpressionBinder(ILogger<SortExpressionBinder> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task BindModelAsync(ModelBindingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                var fieldName = context.FieldName;
                var fieldValueProvider = context.ValueProvider.GetValue(fieldName);
                if (fieldValueProvider.Length == 0)
                {
                    context.Result = ModelBindingResult.Success(new SortExpression(Enumerable.Empty<FieldExpression>()));
                    return Task.CompletedTask;
                }

                var fieldValue = fieldValueProvider.FirstValue;
                if (!ValidationPattern.IsMatch(fieldValue))
                {
                    context.ModelState.AddModelError(fieldName, "The sort expression does not comply with the expected structure");
                    context.Result = ModelBindingResult.Failed();
                    return Task.CompletedTask;
                }

                var model = new SortExpression(ParseFieldExpression(fieldValue));
                context.Result = ModelBindingResult.Success(model);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                context.ModelState.AddModelError(string.Empty, exception, context.ModelMetadata);
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<FieldExpression> ParseFieldExpression(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            var parts = str.Split(',');
            var result = parts.Select(ParseSortDescriptor);
            return result;
        }

        private static FieldExpression ParseSortDescriptor(string str)
        {
            if (str[0] == '-')
            {
                return new FieldExpression
                {
                    FieldName = str.Substring(1),
                    Direction = SortDirection.Descending,
                };
            }
            return new FieldExpression
            {
                FieldName = str,
                Direction = SortDirection.Ascending,
            };
        }
    }
}