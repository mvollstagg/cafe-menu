using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.User.Models
{
    public class DeleteUserModel
    {
        [IOValidationRequired]
        public string Password { get; set; }
    }
}
