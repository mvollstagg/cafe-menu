//using IAndOthers.Api.Attributes;
//using IAndOthers.Application.User.Models;
//using IAndOthers.Core.Data.Enumeration;
//using IAndOthers.Core.Data.Result;
//using IAndOthers.Domain.Entities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace IAndOthers.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class UserController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;

//        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }

//        [HttpGet("me")]
//        [IOApiAuthorize(IOUserRoleEnum.ApplicationUser)]
//        public async Task<IActionResult> GetProfile()
//        {
//            var userId = User.FindFirstValue("id");
//            var user = await _userManager.FindByIdAsync(userId);

//            if (user == null)
//            {
//                if (User.IsInRole(IOUserRoleEnum.Guest.ToString()))
//                {
//                    return Ok(new IOResult<string>(IOResultStatusEnum.Success, "Guest user profile retrieved successfully."));
//                }

//                return NotFound(new IOResult<string>(IOResultStatusEnum.Error, "User not found."));
//            }

//            var userProfile = new
//            {
//                user.FirstName,
//                user.LastName,
//                user.Email,
//                user.UserName,
//                user.ProfileImageUrl
//            };

//            return Ok(new IOResult<object>(IOResultStatusEnum.Success, userProfile, "User profile retrieved successfully."));
//        }

//        [HttpPut("update-profile")]
//        [IOApiAuthorize(IOUserRoleEnum.ApplicationUser)]
//        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
//        {
//            var userId = User.FindFirstValue("id");
//            var user = await _userManager.FindByIdAsync(userId);

//            if (user == null)
//            {
//                return NotFound(new IOResult<string>(IOResultStatusEnum.Error, "User not found"));
//            }

//            user.FirstName = model.FirstName;
//            user.LastName = model.LastName;
//            user.Email = model.Email;
//            user.UserName = model.Email;

//            var result = await _userManager.UpdateAsync(user);
//            if (result.Succeeded)
//            {
//                return Ok(new IOResult<string>(IOResultStatusEnum.Success, "Profile updated successfully"));
//            }

//            var errors = new List<KeyValuePair<string, List<string>>>();

//            foreach (var error in result.Errors)
//            {
//                var existingError = errors.FirstOrDefault(e => e.Key == error.Code);

//                if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
//                {
//                    errors.Add(new KeyValuePair<string, List<string>>(error.Code, new List<string> { error.Description }));
//                }
//                else
//                {
//                    existingError.Value.Add(error.Description);
//                }
//            }

//            return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, null, errors));
//        }

//        [HttpPut("change-password")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
//        {
//            var userId = User.FindFirstValue("id");
//            var user = await _userManager.FindByIdAsync(userId);

//            if (user == null)
//            {
//                return NotFound(new IOResult<string>(IOResultStatusEnum.Error, "User not found"));
//            }

//            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
//            if (result.Succeeded)
//            {
//                return Ok(new IOResult<string>(IOResultStatusEnum.Success, "Password changed successfully"));
//            }

//            var errors = new List<KeyValuePair<string, List<string>>>();

//            foreach (var error in result.Errors)
//            {
//                var existingError = errors.FirstOrDefault(e => e.Key == error.Code);

//                if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
//                {
//                    errors.Add(new KeyValuePair<string, List<string>>(error.Code, new List<string> { error.Description }));
//                }
//                else
//                {
//                    existingError.Value.Add(error.Description);
//                }
//            }

//            return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, null, errors));
//        }

//        [HttpDelete("delete-user")]
//        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserModel model)
//        {
//            var userId = User.FindFirstValue("id");
//            var user = await _userManager.FindByIdAsync(userId);

//            if (user == null)
//            {
//                return NotFound(new IOResult<string>(IOResultStatusEnum.Error, "User not found"));
//            }

//            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
//            if (!passwordCheck)
//            {
//                return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, "Password is incorrect"));
//            }

//            var result = await _userManager.DeleteAsync(user);
//            if (result.Succeeded)
//            {
//                return Ok(new IOResult<string>(IOResultStatusEnum.Success, "User deleted successfully"));
//            }

//            var errors = new List<KeyValuePair<string, List<string>>>();

//            foreach (var error in result.Errors)
//            {
//                var existingError = errors.FirstOrDefault(e => e.Key == error.Code);

//                if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
//                {
//                    errors.Add(new KeyValuePair<string, List<string>>(error.Code, new List<string> { error.Description }));
//                }
//                else
//                {
//                    var updatedError = new KeyValuePair<string, List<string>>(existingError.Key, existingError.Value);
//                    updatedError.Value.Add(error.Description);
//                    errors[errors.IndexOf(existingError)] = updatedError;
//                }
//            }

//            return BadRequest(new IOResult<string>(IOResultStatusEnum.Error, null, errors));
//        }
//    }
//}
