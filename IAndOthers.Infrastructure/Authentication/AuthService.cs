using IAndOthers.Application.Authentication.Interfaces;
using IAndOthers.Application.Authentication.Models;
using IAndOthers.Core.Configs;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IAndOthers.Infrastructure.Authentication
{
    public class AuthService : IAuthService, IIODependencyTransient<IAuthService>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtConfig _jwtConfig;
        private readonly SmtpConfig _smtpConfig;
        private readonly EmailService _emailService;
        private readonly IIORepository<AuthGuestUser, ApplicationDbContext> _authGuestUserRepository;
        private readonly IIORepository<AuthRefreshToken, ApplicationDbContext> _authRefreshTokenRepository;

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           IOptions<JwtConfig> jwtConfig,
                           IOptions<SmtpConfig> smtpConfig,
                           EmailService emailService,
                           IIORepository<AuthGuestUser, ApplicationDbContext> authGuestUserRepository,
                           IIORepository<AuthRefreshToken, ApplicationDbContext> authRefreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtConfig = jwtConfig.Value;
            _smtpConfig = smtpConfig.Value;
            _emailService = emailService;
            _authGuestUserRepository = authGuestUserRepository;
            _authRefreshTokenRepository = authRefreshTokenRepository;
        }

        public async Task<IOResult<AuthResponseDto>> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = new List<KeyValuePair<string, List<string>>>();

                foreach (var error in result.Errors)
                {
                    var existingError = errors.FirstOrDefault(e => e.Key == error.Code);

                    if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
                    {
                        errors.Add(new KeyValuePair<string, List<string>>(error.Code, new List<string> { error.Description }));
                    }
                    else
                    {
                        existingError.Value.Add(error.Description);
                    }
                }

                return new IOResult<AuthResponseDto>(IOResultStatusEnum.Error, null, errors);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendVerificationEmailAsync(user.Email, user.Id, token);

            return new IOResult<AuthResponseDto>(IOResultStatusEnum.Info, "Thank you for registering! Please check your email to verify your account. If you haven’t received the email, be sure to check your spam folder.");
        }

        public async Task<IOResult<AuthResponseDto>> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new IOResult<AuthResponseDto>(IOResultStatusEnum.Error, null, "Invalid login attempt.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new IOResult<AuthResponseDto>(IOResultStatusEnum.Warning, null, "Email is not confirmed. Please check your email.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return new IOResult<AuthResponseDto>(IOResultStatusEnum.Error, null, "Invalid login attempt.");
            }

            return new IOResult<AuthResponseDto>(IOResultStatusEnum.Success, await GenerateJwtToken(new GenerateTokenModel
            {
                Id = user.Id,
                Role = IOUserRoleEnum.ApplicationUser,
                Email = user.Email,
                UserName = user.UserName
            }), "Login successful");
        }

        public async Task<IOResult<string>> ForgotPasswordAsync(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new IOResult<string>(IOResultStatusEnum.Error, null, "User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"https://app-duologue-me.vercel.app/reset-password?email={model.Email}&token={token}";

            var mailMessage = @"
                    <!DOCTYPE html>
                    <html lang=""en"">

                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Reset Your Password</title>
                        <style>
                            body {
                                font-family: 'Helvetica', Arial, sans-serif;
                                background-color: #f4f4f5;
                                color: #333333;
                                margin: 0;
                                padding: 0;
                                -webkit-text-size-adjust: 100%;
                                -ms-text-size-adjust: 100%;
                            }

                            .email-container {
                                background-color: #ffffff;
                                margin: 0 auto;
                                padding: 20px;
                                max-width: 600px;
                                border-radius: 8px;
                                text-align: center;
                            }

                            .logo img {
                                max-width: 100px;
                                margin-bottom: 20px;
                            }

                            h1 {
                                font-size: 24px;
                                font-weight: bold;
                                margin-bottom: 20px;
                                color: #000000;
                                text-align: center;
                            }

                            p {
                                font-size: 16px;
                                line-height: 1.5;
                                margin-bottom: 20px;
                                color: #666666;
                                text-align: center;
                            }

                            .button {
                                display: inline-block;
                                padding: 12px 24px;
                                margin: 20px 0;
                                background-color: #00cc99;
                                color: #ffffff;
                                text-decoration: none;
                                border-radius: 24px;
                                font-weight: bold;
                                text-transform: uppercase;
                            }

                            .footer {
                                font-size: 12px;
                                color: #999999;
                                margin-top: 20px;
                                text-align: center;
                            }

                            .footer a {
                                color: #00cc99;
                                text-decoration: none;
                            }

                            @media only screen and (max-width: 600px) {
                                .email-container {
                                    width: 100% !important;
                                    padding: 15px;
                                }
                            }
                        </style>
                    </head>

                    <body>
                        <div class=""email-container"">
                            <div class=""logo"">
                                <img src=""https://app-duologue-me.vercel.app/assets/logo.png"" alt=""Duologue Me Logo"">
                            </div>
                            <h1>Reset Your Password</h1>
                            <p style=""width: 100%; height: 1px; max-height: 1px; background-color: #d9dbe0; opacity: 0.81""></p>

                            <p>We received a request to reset your password. Click the button below to reset it:</p>
                            <a href=""{{callbackUrl}}"" class=""button"">Reset Password</a>
                            <p>If you didn’t request this, you can safely ignore this email.</p>
                            <div class=""footer"">
                                <p>&copy; 2024 Duologue Me. All rights reserved.</p>
                                <p><a href=""https://app-duologue-me.vercel.app/privacy-policy"">Privacy Policy</a> | <a
                                        href=""https://app-duologue-me.vercel.app/terms-of-service"">Terms of Service</a></p>
                            </div>
                        </div>
                    </body>

                    </html>
            ".Replace("{{callbackUrl}}", callbackUrl);

            await _emailService.SendEmailAsync(user.Email, "Reset Password", mailMessage);

            return new IOResult<string>(IOResultStatusEnum.Info, "Password reset link sent to your email.");
        }

        public async Task<IOResult<string>> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new IOResult<string>(IOResultStatusEnum.Error, null, "User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<KeyValuePair<string, List<string>>>();

                foreach (var error in result.Errors)
                {
                    var existingError = errors.FirstOrDefault(e => e.Key == error.Code);

                    if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
                    {
                        errors.Add(new KeyValuePair<string, List<string>>(error.Code, new List<string> { error.Description }));
                    }
                    else
                    {
                        existingError.Value.Add(error.Description);
                    }
                }

                return new IOResult<string>(IOResultStatusEnum.Error, null, errors);
            }

            return new IOResult<string>(IOResultStatusEnum.Success, "Password has been reset.");
        }

        public async Task<IOResult<AuthResponseDto>> GenerateGuestIdentityAsync()
        {
            var guestUser = new AuthGuestUser
            {
                CreationDateUtc = DateTime.UtcNow
            };
            var result = await _authGuestUserRepository.InsertAsync(guestUser);

            if (result.Status != IOResultStatusEnum.Success)
            {
                var errors = new List<KeyValuePair<string, List<string>>>();

                foreach (var error in result.Messages)
                {
                    var existingError = errors.FirstOrDefault(e => e.Key == error.Key);

                    if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
                    {
                        errors.Add(new KeyValuePair<string, List<string>>(error.Key, new List<string> { error.Value.ToString() }));
                    }
                    else
                    {
                        existingError.Value.Add(error.Value.ToString());
                    }
                }

                return new IOResult<AuthResponseDto>(IOResultStatusEnum.Error, null, errors);
            }

            var authResponse = await GenerateJwtToken(new GenerateTokenModel
            {
                Id = guestUser.Id,
                Role = IOUserRoleEnum.Guest,
                Email = "guest-" + guestUser.Id + "@duologue.me",
                UserName = "guest-" + guestUser.Id
            });
            return new IOResult<AuthResponseDto>(IOResultStatusEnum.Success, authResponse, "Guest user created successfully.");
        }

        private async Task<AuthResponseDto> GenerateJwtToken(GenerateTokenModel user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    // add claim of user type like Guest or ApplicationUser.
                    new Claim(ClaimTypes.Role, Convert.ToInt16(user.Role).ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim("id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);  // Save the refresh token in the database

            return new AuthResponseDto
            {
                Token = jwtToken,
                ExpiresIn = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task SaveRefreshToken(long userId, string refreshToken)
        {
            var newRefreshToken = new AuthRefreshToken
            {
                ApplicationUserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _authRefreshTokenRepository.InsertAsync(newRefreshToken);
        }

        public async Task<IOResult<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenRecord = await _authRefreshTokenRepository
                                            .Table
                                            .Where(x => x.Token == refreshToken &&
                                                        !x.IsRevoked)
                                            .FirstOrDefaultAsync();

            if (!(refreshTokenRecord != null && refreshTokenRecord.ExpiresAt > DateTime.UtcNow))
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            // Invalidate the old refresh token
            await InvalidateRefreshToken(refreshToken);

            var userDetails = await _userManager.FindByIdAsync(refreshTokenRecord.ApplicationUserId.ToString());
            if (userDetails == null)
            {
                throw new SecurityTokenException("User not found");
            }

            // Generate new tokens
            var newJwtToken = await GenerateJwtToken(new GenerateTokenModel
            {
                Role = IOUserRoleEnum.ApplicationUser,
                Id = userDetails.Id,
                Email = userDetails.Email,
                UserName = userDetails.UserName
            });

            return new IOResult<AuthResponseDto>(IOResultStatusEnum.Success, newJwtToken, "Token refreshed successfully");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
                ValidateLifetime = false // Don't validate expiration date here
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task InvalidateRefreshToken(string refreshToken)
        {
            var storedToken = await _authRefreshTokenRepository
                                        .Table
                                        .Where(x => x.Token == refreshToken)
                                        .FirstOrDefaultAsync();

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _authRefreshTokenRepository.UpdateAsync(storedToken);
            }
        }
    }
}
