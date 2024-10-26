using IAndOthers.Core.Configs;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IAndOthers.Admin.Attributes
{
    public class IOApiAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IOUserRoleEnum[] _requiredUserTypes;

        public IOApiAuthorizeAttribute(params IOUserRoleEnum[] requiredUserTypes)
        {
            //IOptions<JwtConfig> jwtConfig
            _jwtConfig = IODependencyResolver.Resolve<IOptions<JwtConfig>>().Value;
            _requiredUserTypes = requiredUserTypes;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var guestUserManager = httpContext.RequestServices.GetRequiredService<IIORepository<AuthGuestUser, ApplicationDbContext>>();

            // Extract the token from the Authorization header
            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Validate token without checking its expiry to determine if it's expired
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // No additional tolerance
                }, out validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                // Token is expired; handle differently
                if (httpContext.Request.Path.Value.Contains("/refresh-token"))
                {
                    // Allow refresh-token endpoint to proceed
                    return;
                }

                // Otherwise, reject the request
                context.Result = new UnauthorizedResult();
                return;
            }
            catch
            {
                // Other validation failures
                context.Result = new UnauthorizedResult();
                return;
            }

            // Proceed with the usual user validation
            var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var isAuthorized = false;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Find guest user
                var guestUser = await guestUserManager
                                        .Table
                                        .Where(u => u.Id == Convert.ToInt64(userId))
                                        .FirstOrDefaultAsync();
                if (guestUser == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                else
                {
                    isAuthorized = _requiredUserTypes.Any(requiredUserType => requiredUserType switch
                    {
                        IOUserRoleEnum.Guest => true,
                        _ => false
                    });
                }
            }
            else
            {
                isAuthorized = _requiredUserTypes.Any(requiredUserType => requiredUserType switch
                {
                    IOUserRoleEnum.Guest => user.Email.Contains("guest.com"),
                    IOUserRoleEnum.ApplicationUser => !user.Email.Contains("guest.com"),
                    _ => false
                });
            }

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
