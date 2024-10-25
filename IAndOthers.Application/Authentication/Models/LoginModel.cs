using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.Authentication.Models
{
    public class LoginModel
    {
        [IOValidationRequired]
        [IOValidationEmail]
        public string Email { get; set; }

        [IOValidationRequired]
        public string Password { get; set; }
    }
}
