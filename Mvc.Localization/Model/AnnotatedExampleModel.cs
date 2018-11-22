using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDemo.Mvc.DataAnnotations.Model
{
    public class AnnotatedExampleModel
    {
        [Range(1, 3, ErrorMessage = "{0} must be a number between {1} and {2}")]
        public int Value { get; set; }
    }
}