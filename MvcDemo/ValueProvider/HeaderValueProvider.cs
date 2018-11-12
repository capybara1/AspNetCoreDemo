using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreDemo.MvcDemo.ValueProvider
{
    public class HeaderValueProvider : IValueProvider
    {
        private readonly IHeaderDictionary _headers;

        public HeaderValueProvider(IHeaderDictionary headers)
        {
            _headers = headers;
        }

        public bool ContainsPrefix(string prefix)
        {
            return _headers.ContainsKey(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            var value = _headers[key].ToString();
            return new ValueProviderResult(value);
        }
    }
}
