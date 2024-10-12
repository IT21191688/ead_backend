// File: EmailService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ead_backend.Services.ServiceImpl
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; // Use 587 for TLS
        private readonly string _serverEmail = "divlinkapp@gmail.com";
        private readonly string _serverPassword = "Uwukuvcaluvcbgri";

        private MailMessage CreateEmailMessage(string email, string subject, string body)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_serverEmail, "E-Com");
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        private async Task SendAsync(MailMessage mailMessage)
        {
            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_serverEmail, _serverPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task SendUserRegisteredEmailAsync(string email, string fullName)
        {
            var subject = "Welcome to E-Com!";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
                    <h1 style='text-align: center; color: #4CAF50;'>Welcome, {fullName}!</h1>
                    <p style='text-align: center; font-size: 18px;'>Thank you for joining our platform. We’re excited to have you on board!</p>
                    <p style='font-size: 16px;'>
                        Explore our wide range of products and enjoy seamless shopping.
                    </p>
                    <p style='font-size: 16px;'>Best regards,<br>E-Com Team</p>
                    <hr style='border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #aaa; text-align: center;'>You received this email because you registered on our platform.</p>
                </div>
            </body>
            </html>";

            var message = CreateEmailMessage(email, subject, body);
            await SendAsync(message);
        }

        public async Task SendOrderStatusEmailAsync(string email, string orderId, string status)
        {
            var subject = $"Order Update: #{orderId}";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
                    <h1 style='text-align: center; color: #2196F3;'>Order Status Update</h1>
                    <p style='font-size: 18px; text-align: center;'>Your order <b>#{orderId}</b> status has been updated to: <b>{status}</b>.</p>
                    <p style='font-size: 16px;'>Thank you for shopping with us!</p>
                    <p style='font-size: 16px;'>Best regards,<br>E-Com Team</p>
                    <hr style='border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #aaa; text-align: center;'>You received this email because you have an active order with us.</p>
                </div>
            </body>
            </html>";

            var message = CreateEmailMessage(email, subject, body);
            await SendAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            var subject = "Password Reset Request";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
                    <h1 style='text-align: center; color: #F44336;'>Password Reset Request</h1>
                    <p style='font-size: 18px; text-align: center;'>We received a request to reset your password. Use the token below to reset your password:</p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 20px; color: #F44336; font-weight: bold;'>{token}</span>
                    </div>
                    <p style='font-size: 16px; text-align: center;'>If you didn’t request a password reset, please ignore this email.</p>
                    <p style='font-size: 16px;'>Best regards,<br>E-Com Team</p>
                    <hr style='border: none; border-top: 1px solid #ddd;'>
                    <p style='font-size: 12px; color: #aaa; text-align: center;'>You received this email because a password reset was requested for your account.</p>
                </div>
            </body>
            </html>";

            var message = CreateEmailMessage(email, subject, body);
            await SendAsync(message);
        }

        public async Task SendActivationRequestToCSREmailAsync(string csrEmail, string fullName, string userEmail)
        {
            var subject = $"Activation Request for New User: {fullName}";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333;'>
        <div style='max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
            <h1 style='text-align: center; color: #FFA500;'>New User Activation Request</h1>
            <p style='font-size: 18px; text-align: center;'>A new user <b>{fullName}</b> has registered with the email <b>{userEmail}</b>.</p>
            <p style='font-size: 16px;'>Please review and activate the user in the system.</p>
            <p style='font-size: 16px;'>Best regards,<br>E-Com Admin Team</p>
            <hr style='border: none; border-top: 1px solid #ddd;'>
            <p style='font-size: 12px; color: #aaa; text-align: center;'>This email is sent to notify you of a new user registration.</p>
        </div>
    </body>
    </html>";

            var message = CreateEmailMessage(csrEmail, subject, body);
            await SendAsync(message);
        }
        public async Task SendOrderCancellationRequestEmailAsync(string orderId,string csrEmail, string fullName, string userEmail)
        {
            var subject = $"Order Cancellation Request";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333;'>
        <div style='max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
            <h1 style='text-align: center; color: #FF6347;'>Order Cancellation Request</h1>
            <p style='font-size: 18px; text-align: center;'>A request has been made to cancel order ID {orderId} <b></b>.</p>
            <p style='font-size: 18px; text-align: center;'>Requested by <b>{fullName}</b> with the email <b>{userEmail}</b>.</p>
            <p style='font-size: 16px;'>Please review and process the cancellation as soon as possible.</p>
            <p style='font-size: 16px;'>Best regards,<br>E-Com Admin Team</p>
            <hr style='border: none; border-top: 1px solid #ddd;'>
            <p style='font-size: 12px; color: #aaa; text-align: center;'>This email is sent to notify you of an order cancellation request.</p>
        </div>
    </body>
    </html>";

            var message = CreateEmailMessage(csrEmail, subject, body);
            await SendAsync(message);
        }

    }
}
