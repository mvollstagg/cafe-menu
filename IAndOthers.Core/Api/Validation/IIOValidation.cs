namespace IAndOthers.Core.Api.Validations
{
    public interface IIOValidation
    {
        Task<string?> IOValidate(object value, object? relatedValue, IServiceProvider serviceProvider);
    }
}
