// File: AuthController
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description: auth api handle

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
        private readonly IEmailService _emailService; // Injecting IEmailService

        public AuthController(AuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService; // Assigning the injected service
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var (success, message, userDetails) = await _authService.RegisterAsync(userDto);
            if (success)
            {
                // Send a registration confirmation email
                await _emailService.SendUserRegisteredEmailAsync(userDetails.Email, userDetails.FirstName + " " + userDetails.LastName);

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
