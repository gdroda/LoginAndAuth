using LoginAndAuth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoginAndAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userServices;

        public AuthController(IUserService userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("login")]
        public async Task<IActionResult> GoogleLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/auth/callback"
            }, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return Unauthorized();

            return Redirect("https://localhost:52565/");
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var name = User.Identity.Name;
                var email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                var user = await _userServices.GetUser(name);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
