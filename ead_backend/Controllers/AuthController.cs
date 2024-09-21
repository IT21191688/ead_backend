using ead_backend.Model.Dtos;
using ead_backend.Services;
using ead_backend.Utills;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var (success, message, userDetails) = await _authService.RegisterAsync(userDto);
            if (success)
            {
                return this.CustomResponse(true, 201, message, userDetails);
            }
            else
            {
                return this.CustomResponse(false, 400, message, null);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var (success, message, token, userDetails) = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
            if (success)
            {
                return this.CustomResponse(true, 200, message, new { Token = token, User = userDetails });
            }
            else
            {
                return this.CustomResponse(false, 401, message, null);
            }
        }
    }
}