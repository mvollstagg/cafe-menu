using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.Authentication.Models
{
    public class RegisterModel
    {
        [IOValidationRequired]
        [IOValidationMinLength(2)]
        public string FirstName { get; set; }

        [IOValidationRequired]
        [IOValidationMinLength(2)]
        public string LastName { get; set; }

        [IOValidationRequired]
        [IOValidationEmail]
        public string Email { get; set; }

        [IOValidationRequired]
        [IOValidationMinLength(7)]
        public string Password { get; set; }

        [IOValidationRequired]
        [IOValidationMustBeSame(nameof(Password))]
        public string PasswordAgain { get; set; }

        [IOValidationMustBeTrue("You must agree to the terms and conditions.")]
        public bool AgreeToTerms { get; set; }
    }
}
