using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDemo.MvcDemo.Model
{
    public class SetExampleInputModel
    {
        [FromRoute]
        [MaxLength(10, ErrorMessage = "The name cannot be longer than 10 characters")]
        [BindRequired]
        public string Name { get; set; }

        [FromBody]
        [BindRequired]
        public ExampleModel Example { get; set; }
    }
}