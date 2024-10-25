namespace IAndOthers.Core.Api.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IOValidationMustBeSameAttribute : Attribute, IIOValidation
    {
        public string RelatedProperty { get; }
        public string ErrorMessage { get; }

        public IOValidationMustBeSameAttribute(string relatedProperty, string errorMessage = "The fields do not match.")
        {
            RelatedProperty = relatedProperty;
            ErrorMessage = errorMessage;
        }

        public async Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider)
        {
            if (value == null || relatedValue == null)
            {
                return ErrorMessage;
            }

            if (!string.Equals(value.ToString(), relatedValue.ToString(), StringComparison.Ordinal))
            {
                return ErrorMessage;
            }

            return null;
        }
    }
}
