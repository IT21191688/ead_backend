namespace ead_backend.Services
{
    public interface IEmailService
    {
        Task SendUserRegisteredEmailAsync(string email, string fullName);
        Task SendOrderStatusEmailAsync(string email, string orderId, string status);
        Task SendPasswordResetEmailAsync(string email, string token);
    }
}
