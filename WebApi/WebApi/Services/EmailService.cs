using MailKit.Net.Smtp;
using MailKit.Security; 
using MimeKit;
using WebApi.Services;
using WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.InteropServices;

namespace WebApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task RegisterEmail(User user, string verifyUrl);
        Task ActivationEmail(Candidate candidate, string url);
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
                // var enableSsl = bool.Parse(_confi["EmailSettings:EnableSsl"]);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("RPMS",senderEmail));
                message.To.Add(new MailboxAddress("",to));
                message.Subject = subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using(var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }

        }
        public async Task RegisterEmail(User user, string verifyUrl)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "Register.html");
            string body = File.ReadAllText(templatePath);

            body = body.Replace("{{Token}}", user.VerifyToken)
                       .Replace("{{Email}}", user.Email)
                       .Replace("{{Year}}", DateTime.Now.Year.ToString())
                       .Replace("{{AppUrl}}", verifyUrl);
            await SendEmailAsync(user.Email, "Welcome to Our Platform", body);
        }
        public async Task ActivationEmail(Candidate candidate, string url)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "Activation.html");
            string body = File.ReadAllText(templatePath);
            body = body.Replace("{{Email}}", candidate.Email)
                .Replace("{{Password}}", candidate.PasswordHash)
                .Replace("{{LoginUrl}}", url)
                .Replace("{{Year}}", DateTime.Now.Year.ToString());
            await SendEmailAsync(candidate.Email, "Credential Delivery", body);

        }
    }
}