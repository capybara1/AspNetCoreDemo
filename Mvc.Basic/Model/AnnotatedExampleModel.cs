using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDemo.Mvc.Basic.Model
{
    public class AnnotatedExampleModel
    {
        [BindRequired]
        [StringLength(4)]
        public string Value { get; set; }

        [Range(1, 3, ErrorMessage = "Priority must be a number between 1 and 3")]
        public int Priority { get; set; }

        [BindRequired]
        [RegularExpression("^[a-z]", ErrorMessage = "The text must start with a lower case letter")]
        public string Text { get; set; }
    }
}