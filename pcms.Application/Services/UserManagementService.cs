using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Domain.Config;
using pcms.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pcms.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(ILogger<UserManagementService> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<string>> Register(RegisterUser registerUser)
        {
            var response = new ApiResponse<string>();
            try
            {
                var userExists = await _userManager.FindByEmailAsync(registerUser.Email);
                if (userExists != null)
                {
                    response.ResponseCode = "01";
                    response.ResponseMessage = "Cannot create an account with this email. ";
                    return response;
                }
                if (!(await _roleManager.RoleExistsAsync(registerUser.UserRole.ToString())))
                {
                    response.ResponseCode = "01";
                    response.ResponseMessage = "Invalid role specified";
                    return response;
                }

                IdentityUser identityUser = new IdentityUser
                {
                    Email = registerUser.Email,
                    SecurityStamp = System.Guid.NewGuid().ToString(),
                    UserName = registerUser.UserName

                };
                var res = await _userManager.CreateAsync(identityUser, registerUser.Password);
                if (res.Succeeded)
                {
                    var roleRes = await _userManager.AddToRoleAsync(identityUser, registerUser.UserRole.ToString());
                    //var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    //if (token != null)
                    //{
                    //    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new {token, identityUser.Email});
                    //    var message = new Message(new string[] { identityUser.Email! }, "Confirmation email link", confirmationLink);
                    //    _emailService.SendEmail(message);
                    //}

                    response.ResponseCode = "00";
                    response.ResponseMessage = "User created successfully";
                }

            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponse<tokenDto>> Login(UserLogin userLogin)
        {
            var response = new ApiResponse<tokenDto>();
            try
            {
                //Check if user exists
                var validUser = await _userManager.FindByNameAsync(userLogin.UserName);
                if (validUser == null)
                {
                    response.ResponseCode = "01";
                    response.ResponseMessage = "User not found";
                    return response;
                }
                var validPass = await _userManager.CheckPasswordAsync(validUser, userLogin.Password);
                if (!validPass)
                {
                    response.ResponseCode = "01";
                    response.ResponseMessage = "You have entered an invalid username or password";
                    return response;
                }
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userLogin.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var roles = await _userManager.GetRolesAsync(validUser);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var token = GetToken(claims);
                response.Data = new tokenDto { access_token = new JwtSecurityTokenHandler().WriteToken(token), expiry = (int)(token.ValidTo - DateTime.Now).TotalSeconds };
                response.ResponseCode = "00";
                response.ResponseMessage = "Success";
                return response;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigSettings.Jwt.Secret));

            var token = new JwtSecurityToken(

                issuer: ConfigSettings.Jwt.Secret,
                audience: ConfigSettings.Jwt.Secret,
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
    }
}
