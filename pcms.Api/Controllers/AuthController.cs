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
        private readonly IUserManagementService _userManagementService;
        private readonly ModelValidationService _validationService;
        // private readonly UserManagement.Service.Email.IEmailService _emailService;

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IUserManagementService userManagementService, ModelValidationService validationService)
        {
            _logger = logger;
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
                return Ok(await _userManagementService.Register(registerUser));
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
    }
}