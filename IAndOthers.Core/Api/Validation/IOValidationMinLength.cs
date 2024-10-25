namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationMinLengthAttribute : Attribute, IIOValidation
    {
        public int MinLength { get; }
        public string ErrorMessage { get; }

        public IOValidationMinLengthAttribute(int minLength, string errorMessage = null)
        {
            MinLength = minLength;
            ErrorMessage = errorMessage ?? $"The field length must be greater than or equal to {MinLength} characters.";
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value != null && value.ToString().Length < MinLength)
            {
                return ErrorMessage;
            }
            return null;
        }
    }
}
