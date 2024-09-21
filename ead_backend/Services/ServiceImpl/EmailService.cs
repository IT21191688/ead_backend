namespace ead_backend.Services.ServiceImpl
{
    public class EmailService : IEmailService
    {
        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            // Implement email sending logic here
            // For now, we'll just simulate sending an email
            await Task.CompletedTask;
            Console.WriteLine($"Password reset email sent to {email} with token: {token}");
        }
    }
}