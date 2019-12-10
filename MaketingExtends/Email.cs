using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace MaketingExtends
{
    public class Email : IEmail
    {
        private readonly EmailSettings _emailSettings;
        public Email(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }
        public Email(EmailSettings settings)
        {
            _emailSettings = settings;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                     await smtpClient.SendMailAsync(new MailMessage(_emailSettings.Sender,email,subject,message));
                }
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

    }
}
