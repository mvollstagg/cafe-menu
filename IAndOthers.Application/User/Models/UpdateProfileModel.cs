using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.User.Models
{
    public class UpdateProfileModel
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
    }
}
