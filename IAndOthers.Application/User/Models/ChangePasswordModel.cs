using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.User.Models
{
    public class ChangePasswordModel
    {
        [IOValidationRequired]
        public string CurrentPassword { get; set; }

        [IOValidationRequired]
        [IOValidationMinLength(7)]
        public string NewPassword { get; set; }

        [IOValidationRequired]
        [IOValidationMustBeSame(nameof(NewPassword))]
        public string NewPasswordAgain { get; set; }
    }
}
