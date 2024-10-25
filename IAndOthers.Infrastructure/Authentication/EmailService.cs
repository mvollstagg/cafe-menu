using IAndOthers.Core.Configs;
using IAndOthers.Core.IoC;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IAndOthers.Infrastructure.Authentication
{
    public class EmailService : IIODependencyTransient
    {
        private readonly SmtpConfig _smtpConfig;

        public EmailService(IOptions<SmtpConfig> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_smtpConfig.Password); // Password is your SendGrid API Key
            var from = new EmailAddress(_smtpConfig.SenderEmail, "Duologue");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);
            // Handle response if necessary
        }

        public async Task SendVerificationEmailAsync(string toEmail, long userId, string token)
        {
            var verificationLink = $"https://app-duologue-me.vercel.app/verify-email?userId={userId}&token={Uri.EscapeDataString(token)}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking on the link: <a href='{verificationLink}'>Verify Email</a>";

            await SendEmailAsync(toEmail, subject, message);
        }

    }
}
