namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationMaxLengthAttribute : Attribute, IIOValidation
    {
        public int MaxLength { get; }
        public string ErrorMessage { get; }

        public IOValidationMaxLengthAttribute(int maxLength, string errorMessage = null)
        {
            MaxLength = maxLength;
            ErrorMessage = errorMessage ?? $"The field length must be less than or equal to {MaxLength} characters.";
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value != null && value.ToString().Length > MaxLength)
            {
                return ErrorMessage;
            }
            return null;
        }
    }
}
