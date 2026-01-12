using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using WebApi.Services;
using WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task RegisterEmail(User user, string verifyUrl);
        Task ActivationEmail(Candidate candidate, string url);
        Task ApplicationAknow(string name, string email, string title, string date);
        Task Shortlisted(string name, string email, string title, string date);
        Task InterviewScheduled(string candidateName, string email, string positionName, string roundName, DateTime scheduledAt, string mode, string locationOrLink);

        Task InterviewResult(string candidateName, string email, string positionName, string roundName, string result, string remarks);
        Task SelectedCandidateEmail(string candidateName, string email, string positionName);
        Task RejectedCandidateEmail(string candidateName, string email, string positionName);
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
                message.From.Add(new MailboxAddress("RPMS", senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
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
        public async Task ApplicationAknow(string name, string email, string title, string date)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "ApplicationAknow.html");
            string body = File.ReadAllText(templatePath);
            body = body.Replace("{{CandidateName}}", name)
                .Replace("{{JobTitle}}", title)
                .Replace("{{ApplicationDate}}", date.ToString())
                .Replace("{{Year}}", DateTime.Now.Year.ToString());
            await SendEmailAsync(email, "Job Application Acknowledgment", body);
        }

        public async Task Shortlisted(string name, string email, string title, string date)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "Shortlisted.html");
            string body = File.ReadAllText(templatePath);
            body = body.Replace("{{CandidateName}}", name)
            .Replace("{{JobTitle}}", title)
                .Replace("{{ApplicationDate}}", date.ToString())
                .Replace("{{Year}}", DateTime.Now.Year.ToString());
            await SendEmailAsync(email, "Application has been shortlisted", body);
        }

        public async Task InterviewScheduled(string candidateName, string email, string positionName, string roundName, DateTime scheduledAt, string mode, string locationOrLink)
        {
            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Template",
                "InterviewSchedule.html"
            );

            string body = File.ReadAllText(templatePath);

            body = body.Replace("{{CandidateName}}", candidateName)
                       .Replace("{{PositionName}}", positionName)
                       .Replace("{{RoundName}}", roundName)
                       .Replace("{{Date}}", scheduledAt.ToString("dd MMM yyyy"))
                       .Replace("{{Time}}", scheduledAt.ToString("hh:mm tt"))
                       .Replace("{{Mode}}", mode)
                       .Replace("{{Location}}", locationOrLink)
                       .Replace("{{MeetingLink}}",
                           mode == "Online" ? locationOrLink : "#")
                       .Replace("{{Year}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(
                email,
                $"Interview Scheduled – {positionName}",
                body
            );
        }

        public async Task InterviewResult(string candidateName, string email, string positionName, string roundName, string result, string remarks)
        {
            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Template",
                "InterviewResult.html"
            );

            string body = File.ReadAllText(templatePath);

            string nextStep = result == "Selected"
                ? "Congratulations! Our HR team will contact you shortly regarding the next steps."
                : "We encourage you to apply for future opportunities with us.";

            body = body.Replace("{{CandidateName}}", candidateName)
                       .Replace("{{PositionName}}", positionName)
                       .Replace("{{RoundName}}", roundName)
                       .Replace("{{Result}}", result)
                       .Replace("{{Remarks}}", remarks)
                       .Replace("{{NextStep}}", nextStep)
                       .Replace("{{Year}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(
                email,
                $"Interview Result – {positionName}",
                body
            );
        }
        public async Task SelectedCandidateEmail(string candidateName, string email, string positionName)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "SelectionMail.html");
            string body = File.ReadAllText(templatePath);
            
            body = body.Replace("{{CandidateName}}", candidateName)
                       .Replace("{{PositionName}}", positionName)
                       .Replace("{{Date}}", DateTime.Now.ToString("dd MM yyyy"))
                       .Replace("{{Year}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(email, $"Congratulations! You are selected – {positionName}", body);
        }

        public async Task RejectedCandidateEmail(string candidateName, string email, string positionName)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "RejectionMail.html");
            string body = File.ReadAllText(templatePath);

            body = body.Replace("{{CandidateName}}", candidateName)
                       .Replace("{{PositionName}}", positionName)
                       .Replace("{{Year}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(email, $"Application Update – {positionName}", body);
        }

    }
}