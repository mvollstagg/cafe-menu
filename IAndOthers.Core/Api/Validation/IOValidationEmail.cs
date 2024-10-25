using System.Text.RegularExpressions;

namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationEmailAttribute : Attribute, IIOValidation
    {
        public string ErrorMessage { get; }

        private static readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public IOValidationEmailAttribute(string errorMessage = "Invalid email format.")
        {
            ErrorMessage = errorMessage;
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value == null || !_emailRegex.IsMatch(value.ToString()))
            {
                return ErrorMessage;
            }
            return null;
        }
    }
}
