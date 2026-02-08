using LoginAndAuth.Data;
using LoginAndAuth.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LoginAndAuth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly TokenServices _tokenServices;
        private readonly IUserService _userServices;
        public AuthController(TokenServices tokenServices, IUserService userServices)
        {
            _tokenServices = tokenServices;
            _userServices = userServices;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest request)
        {
            var response = _userServices.GetUser(request.Email);
            if (response != null)
            {
                var token = _tokenServices.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        [HttpPost("google")]
        public async Task<IActionResult> GetTokenGoogle([FromBody] LoginRequest request)
        {
            var response = _userServices.GetUser(request.Email);
            if (response != null)
            {
                var token = _tokenServices.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }
    }
}
