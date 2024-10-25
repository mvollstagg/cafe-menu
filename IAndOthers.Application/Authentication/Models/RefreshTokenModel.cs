using IAndOthers.Core.Api.Validations;

namespace IAndOthers.Application.Authentication.Models
{
    public class RefreshTokenModel
    {
        [IOValidationRequired]
        public string RefreshToken { get; set; }
    }
}
