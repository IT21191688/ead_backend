using ead_backend.Data;
using ead_backend.Healpers;
using ead_backend.Model;
using ead_backend.Model.Dtos;
using ead_backend.Services;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ead_backend.Services.ServiceImpl
{
    public class UserService : IUserService
    {
        //private readonly IMongoCollection<User> _users;

        //public UserService(MongoDbContext context)
        //{
        //    _users = context.Users;
        //}

        private readonly IMongoCollection<User> _users;
        private readonly IEmailService _emailService;

        public UserService(MongoDbContext context, IEmailService emailService)
        {
            _users = context.Users;
            _emailService = emailService;
        }



        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            var existingUser = await _users.Find(u => u.Email == userDto.Email).FirstOrDefaultAsync();
            if (existingUser != null) return null;

            var hashedPassword = PasswordHasher.HashPassword(userDto.Password);
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Age = userDto.Age,
                PasswordHash = hashedPassword,
                Role = userDto.Role,
                Status = userDto.Status,
            };

            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, password)) return null;
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> UpdateUserAsync(string email, UserUpdateDto userUpdateDto)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return null;

            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.Age = userUpdateDto.Age;
            user.Status = userUpdateDto.Status;

            await _users.ReplaceOneAsync(u => u.Email == email, user);
            return user;
        }

        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await AuthenticateUserAsync(email, currentPassword);
            if (user == null) return false;

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _users.ReplaceOneAsync(u => u.Email == email, user);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null || user.ResetPasswordToken != token || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _users.ReplaceOneAsync(u => u.Email == email, user);
            return true;
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return false;

            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _users.ReplaceOneAsync(u => u.Email == email, user);

            // Send email with reset token
            await _emailService.SendPasswordResetEmailAsync(email, token);

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _users.Find(_ => true).ToListAsync();
            return users.Select(u => new UserDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Age = u.Age,
                Role = u.Role,
                Status = u.Status,
            });
        }

        public async Task<bool> IsUserAdminAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            return user != null && user.Role.ToLower() == "admin";
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var result = await _users.DeleteOneAsync(u => u.Email == email);
            return result.DeletedCount > 0;
        }
    }
}