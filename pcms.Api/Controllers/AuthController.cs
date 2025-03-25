using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using pcms.Application.Interfaces;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
       
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserManagementService _userManagementService;
        private readonly ModelValidationService _validationService;
        // private readonly UserManagement.Service.Email.IEmailService _emailService;

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserManagementService userManagementService, ModelValidationService validationService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userManagementService = userManagementService;
            _validationService = validationService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterUser registerUser)
        {
            try
            {
                var validationResult = _validationService.Validate(registerUser);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.customProblemDetail.Detail);
                }
                return Ok(_userManagementService.Register(registerUser));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            try
            {
                var validationResult = _validationService.Validate(userLogin);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.customProblemDetail.Detail);
                }
                return Ok(await _userManagementService.Login(userLogin));              
            }
            catch (Exception)
            {
                throw;
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(

                issuer: _configuration["JWT:Secret"],
                audience: _configuration["JWT:Secret"],
                expires: DateTime.Now.AddHours(1),
                claims : authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
    }
}