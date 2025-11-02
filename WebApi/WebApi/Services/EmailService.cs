using System.Net.Mail;
using System.Net;
using WebApi.Services;
using WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task RegisterEmail(User user);
    }
    public class EmailService : IEmailService
    {
       private readonly IConfiguration _confi;
       private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration confi, ILogger<EmailService> logger)
        {
            _confi = confi;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpServer = _confi["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_confi["EmailSettings:SmtpPort"]);
                var senderEmail = _confi["EmailSettings:SenderEmail"];
                var senderPassword = _confi["EmailSettings:SenderPassword"];
                var enableSsl = bool.Parse(_confi["EmailSettings:EnableSsl"]);

                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(senderEmail);
                        mailMessage.To.Add(to);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }

        }
        public async Task RegisterEmail(User user)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "Register.html");
            string body = File.ReadAllText(templatePath);

            body = body.Replace("{{Token}}", user.VerifyToken)
                       .Replace("{{Email}}", user.Email)
                       .Replace("{{Year}}", DateTime.Now.Year.ToString());
            await SendEmailAsync(user.Email, "Welcome to Our Platform", body);
        }
    }
}