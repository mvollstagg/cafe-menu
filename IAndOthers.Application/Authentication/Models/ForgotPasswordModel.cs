using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.Authentication.Models
{
    public class ForgotPasswordModel
    {
        [IOValidationRequired]
        [IOValidationEmail]
        public string Email { get; set; }
    }
}
