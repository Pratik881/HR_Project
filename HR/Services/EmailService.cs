using HR.Interfaces;
using System.Net;
using System.Net.Mail;

namespace HR.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                    ),
                EnableSsl = true
            };
            var mailMessage=new MailMessage
            {
                From=new MailAddress(_configuration["EmailSettings:SenderEmail"]),
                Subject=subject,
                Body=body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
