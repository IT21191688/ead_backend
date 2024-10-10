// File: IEmailService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

namespace ead_backend.Services
{
    public interface IEmailService
    {
        Task SendUserRegisteredEmailAsync(string email, string fullName);
        Task SendOrderStatusEmailAsync(string email, string orderId, string status);
        Task SendPasswordResetEmailAsync(string email, string token);
        Task SendActivationRequestToCSREmailAsync(string csrEmail, string fullName, string userEmail);
    }
}
