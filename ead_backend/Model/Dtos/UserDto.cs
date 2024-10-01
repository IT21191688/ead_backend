namespace ead_backend.Model.Dtos
{
    public class UserDto
    {
        public string? id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public string Status { get; set; }
    }

    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; }
    }

}
