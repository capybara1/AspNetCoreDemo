using System.Text.RegularExpressions;

namespace AspNetCoreDemo.Utils.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex ControllerSuffix = new Regex(@"Controller$");

        public static string RemoveControllerSuffix(this string value)
        {
            if (value == null) return null;

            var result = ControllerSuffix.Replace(value, string.Empty);

            return result;
        }
    }
}
