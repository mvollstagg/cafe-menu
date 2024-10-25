//using IAndOthers.Api.Attributes;
//using IAndOthers.Application.Authentication.Interfaces;
//using IAndOthers.Application.Authentication.Models;
//using IAndOthers.Core.Data.Enumeration;
//using IAndOthers.Core.Data.Result;
//using IAndOthers.Domain.Entities;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace IAndOthers.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
//        {
//            _authService = authService;
//            _userManager = userManager;
//        }

//        [HttpGet("guest")]
//        public async Task<IActionResult> GenerateGuestIdentity()
//        {
//            var result = await _authService.GenerateGuestIdentityAsync();
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpPost("register")]
//        [IOApiAuthorize(IOUserRoleEnum.Guest)]
//        public async Task<IActionResult> Register([FromBody] RegisterModel model)
//        {
//            var result = await _authService.RegisterAsync(model);
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpPost("login")]
//        [IOApiAuthorize(IOUserRoleEnum.Guest)]
//        public async Task<IActionResult> Login([FromBody] LoginModel model)
//        {
//            var result = await _authService.LoginAsync(model);
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpPost("refresh-token")]
//        [IOApiAuthorize(IOUserRoleEnum.Guest, IOUserRoleEnum.ApplicationUser)]
//        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
//        {
//            var result = await _authService.RefreshTokenAsync(model.RefreshToken);
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpPost("forgot-password")]
//        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
//        {
//            var result = await _authService.ForgotPasswordAsync(model);
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpPost("reset-password")]
//        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
//        {
//            var result = await _authService.ResetPasswordAsync(model);
//            if (result.Meta.Status == IOResultStatusEnum.Error)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpGet("verify-email")]
//        public async Task<IActionResult> VerifyEmail(string userId, string token)
//        {
//            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
//            {
//                return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, "Invalid email verification request."));
//            }

//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, "Invalid user."));
//            }

//            var result = await _userManager.ConfirmEmailAsync(user, token);
//            if (!result.Succeeded)
//            {
//                return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, "Email verification failed."));
//            }

//            return Ok(new IOResult<string>(IOResultStatusEnum.Success, "Email verified successfully."));
//        }
//    }
//}
