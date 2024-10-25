namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationMustBeTrueAttribute : Attribute, IIOValidation
    {
        public string ErrorMessage { get; }

        public IOValidationMustBeTrueAttribute(string errorMessage = "This field must be true.")
        {
            ErrorMessage = errorMessage;
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value is bool booleanValue && !booleanValue)
            {
                return ErrorMessage;
            }
            return null;
        }
    }
}
