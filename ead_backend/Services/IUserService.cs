//using login_app.Model;
//using login_app.Model.Dtos;
//using System.Threading.Tasks;

//namespace login_app.Services
//{
//    public interface IUserService
//    {
//        Task<User> CreateUserAsync(UserDto userDto);
//        Task<User> AuthenticateUserAsync(string email, string password);
//        Task<User> GetUserByEmailAsync(string email);
//    }
//}

using ead_backend.Model.Dtos;
using ead_backend.Model;

namespace ead_backend.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserDto userDto);
        Task<User> AuthenticateUserAsync(string email, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> UpdateUserAsync(string email, UserUpdateDto userUpdateDto);
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> InitiatePasswordResetAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> IsUserAdminAsync(string email);
        Task<bool> DeleteUserAsync(string email);
    }
}