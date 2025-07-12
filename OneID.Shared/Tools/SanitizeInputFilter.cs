using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable
namespace OneID.Shared.Tools
{
    public sealed class SanitizeInputFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is string stringValue)
                {
                    context.ActionArguments[argument.Key] = SanitizeString(stringValue);
                }

                if (argument.Value != null && argument.Value.GetType().IsClass)
                {
                    SanitizeObjectProperties(argument.Value);
                }
            }
        }
        private static string SanitizeString(string input)
        {
            string noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            return noHtml
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#x27;")
                .Replace("/", "&#x2F;");
        }

        private static void SanitizeObjectProperties(object obj)
        {
            if (obj == null) return;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var value = property.GetValue(obj);

                if (property.PropertyType == typeof(string) && value is string str && !string.IsNullOrEmpty(str))
                {
                    property.SetValue(obj, SanitizeString(str));
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    SanitizeObjectProperties(value);
                }
            }
        }
    }
}
