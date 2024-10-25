namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationRequiredAttribute : Attribute, IIOValidation
    {
        public string ErrorMessage { get; }

        public IOValidationRequiredAttribute(string errorMessage = "This field is required.")
        {
            ErrorMessage = errorMessage;
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ErrorMessage;
            }
            return null;
        }
    }
}
