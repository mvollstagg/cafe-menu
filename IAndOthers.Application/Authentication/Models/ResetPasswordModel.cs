using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.Authentication.Models
{
    public class ResetPasswordModel
    {
        [IOValidationRequired]
        [IOValidationEmail]
        public string Email { get; set; }

        [IOValidationRequired]
        public string Token { get; set; }

        [IOValidationRequired]
        [IOValidationMinLength(7)]
        public string NewPassword { get; set; }

        [IOValidationRequired]
        [IOValidationMustBeSame(nameof(NewPassword))]
        public string NewPasswordAgain { get; set; }
    }
}
