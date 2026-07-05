using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MailKit.Security;
namespace TaskManagementSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LoadTemplate(string templateName)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "EmailTemplates",
                templateName);

            return File.ReadAllText(path);
        }
        public async Task SendEmail(string to,string subject,string body)

        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:Email"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                 _configuration["EmailSettings:Host"],
                 int.Parse(_configuration["EmailSettings:Port"]!),
                 SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Email"],
                _configuration["EmailSettings:Password"]);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
        

    }
}
