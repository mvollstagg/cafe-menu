using IAndOthers.Core.Api.Validations;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Application.Authentication.Models
{
    public class LoginModel
    {
        [IOValidationRequired]
        [IOValidationEmail]
        [Display(Name = "Username")]
        public string Email { get; set; }

        [IOValidationRequired]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
