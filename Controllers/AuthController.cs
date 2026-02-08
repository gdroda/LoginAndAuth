using Google.Apis.Auth;
using LoginAndAuth.Models;
using LoginAndAuth.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LoginAndAuth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly TokenServices _tokenServices;
        private readonly IUserService _userServices;
        private readonly IConfiguration _configuration;
        public AuthController(TokenServices tokenServices, IUserService userServices, IConfiguration configuration)
        {
            _tokenServices = tokenServices;
            _userServices = userServices;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest request)
        {
            var response = await _userServices.GetUser(request.Email);
            if (response != null)
            {
                var token = await _tokenServices.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        [HttpPost("google")]
        public async Task<IActionResult> GetTokenGoogle([FromBody] GoogleLoginRequest request)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            var email = payload.Email;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var user = await _userServices.GetUser(email);
            if (user == null)
            {
                var newUserDTO = new UserDTO { Name = payload.Name ?? email, Email = email };
                await _userServices.CreateUser(newUserDTO);
            }
            var token = await _tokenServices.GenerateToken(email);
            return Ok(new { Token = token });
        }
    }
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
