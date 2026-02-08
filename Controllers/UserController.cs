using LoginAndAuth.Models;
using LoginAndAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginAndAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser (UserDTO userDTO)
        {
            var response = await _userService.CreateUser(userDTO);
            if (response != null) return Ok(response);
            else return BadRequest();
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userName)
        {
            var response = await _userService.GetUser(userName);
            if (response != null) return Ok(response);
            else return NotFound();
        }
    }
}
