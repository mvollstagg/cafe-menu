using IAndOthers.Application.Authentication.Models;
using IAndOthers.Core.Data.Result;

namespace IAndOthers.Application.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<IOResult<AuthResponseDto>> RegisterAsync(RegisterModel model);
        Task<IOResult<AuthResponseDto>> LoginAsync(LoginModel model);
        Task<IOResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<IOResult<string>> ForgotPasswordAsync(ForgotPasswordModel model);
        Task<IOResult<string>> ResetPasswordAsync(ResetPasswordModel model);
        Task<IOResult<AuthResponseDto>> GenerateGuestIdentityAsync();
    }
}