// File: UserController
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description: Handle all api user apis

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using ead_backend.Services;
using ead_backend.Model.Dtos;
using ead_backend.Utills;
using ead_backend.Healpers;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;

        public UserController(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return this.CustomResponse(false, 404, "User not found", null);
            }

            var userDto = new UserDto
            {
                id=user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Age = user.Age,
                Role = user.Role,
                Status = user.Status,
            };

            return this.CustomResponse(true, 200, "Profile retrieved successfully", userDto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updatedUser = await _userService.UpdateUserAsync(userEmail, userUpdateDto);

            if (updatedUser == null)
            {
                return this.CustomResponse(false, 404, "User not found", null);
            }

            var userDto = new UserDto
            {
                id=updatedUser.Id.ToString(),
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email,
                Age = updatedUser.Age,
                Role = updatedUser.Role,
                Status = updatedUser.Status,
            };

            return this.CustomResponse(true, 200, "Profile updated successfully", userDto);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userService.ChangePasswordAsync(userEmail, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result)
            {
                return this.CustomResponse(false, 400, "Failed to change password", null);
            }

            return this.CustomResponse(true, 200, "Password changed successfully", null);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result)
            {
                return this.CustomResponse(false, 400, "Failed to reset password", null);
            }

            return this.CustomResponse(true, 200, "Password reset successfully", null);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _userService.InitiatePasswordResetAsync(forgotPasswordDto.Email);

            if (!result)
            {
                return this.CustomResponse(false, 400, "Failed to initiate password reset", null);
            }

            return this.CustomResponse(true, 200, "Password reset initiated. Check your email for further instructions.", null);
        }

        [HttpGet("get-all-users")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return this.CustomResponse(true, 200, "Users retrieved successfully", users);
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var adminEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = await _userService.IsUserAdminAsync(adminEmail);

            if (!isAdmin)
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var result = await _userService.DeleteUserAsync(email);

            if (!result)
            {
                return this.CustomResponse(false, 404, "User not found", null);
            }

            return this.CustomResponse(true, 200, "User deleted successfully", null);
        }

        [HttpPut("update-status/{userId}")]
        [Authorize(Roles = "csr")]
        public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] UpdateStatusDto updateStatusDto)
        {
            if (string.IsNullOrEmpty(updateStatusDto?.Status))
            {
                return this.CustomResponse(false, 400, "The status field is required.", null);
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.CustomResponse(false, 404, "User not found", null);
            }

            user.Status = updateStatusDto.Status;

            var updatedUser = await _userService.UpdateUserStatusAsync(userId, updateStatusDto.Status);

            if (updatedUser == null)
            {
                return this.CustomResponse(false, 500, "Failed to update user status", null);
            }
            var userDto = new UserDto
            {
                id =updatedUser.Id.ToString(),
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email,
                Age = updatedUser.Age,
                Role = updatedUser.Role,
                Status = updatedUser.Status,
            };

            return this.CustomResponse(true, 200, "User status updated successfully", userDto);
        }



    }
}