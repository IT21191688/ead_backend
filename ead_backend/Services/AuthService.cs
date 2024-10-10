// File: AuthService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using ead_backend.Healpers;
using ead_backend.Model;
using ead_backend.Model.Dtos;
using System.Threading.Tasks;

namespace ead_backend.Services
{
    public class AuthService
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        public async Task<(bool Success, string Message, UserDto UserDetails)> RegisterAsync(UserDto userDto)
        {
            var user = await _userService.CreateUserAsync(userDto);
            if (user == null) return (false, "User already exists.", null);

            var userDetails = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Age = user.Age,
                Role = user.Role,
                Status = user.Status,
            };

            return (true, "User registered successfully.", userDetails);
        }

        public async Task<(bool Success, string Message, string Token, UserDto UserDetails)> LoginAsync(string email, string password)
        {
            var user = await _userService.AuthenticateUserAsync(email, password);
            if (user == null) return (false, "Invalid credentials.", null, null);

            var token = _jwtHelper.GenerateJwtToken(user.Email, user.Role,user.Id.ToString());

            var userDetails = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Age = user.Age,
                Role = user.Role,
                Status = user.Status,
            };
            return (true, "Login successful.", token, userDetails);
        }


        public async Task<List<string>> GetCsrEmailsAsync()
        {
            // Get the list of users with the CSR role
            var users = await _userService.GetUsersByRoleAsync();

            // Extract the emails from the list of User objects
            var csrEmails = users.Select(u => u.Email).ToList();
            return csrEmails;
        }


    }
}