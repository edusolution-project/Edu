using System;
using System.Net;
using System.Net.Mail;

namespace BaseMVC.Globals
{
    public class Mail
    {
        public Mail()
        {
            _emailSend = "mylovebroken942@gmail.com";
            _passWord = "04091994long";
        }
        private readonly string _emailSend;
        private readonly string _passWord;
        public Mail(string email, string pass)
        {
            _emailSend = email;
            _passWord = pass;
        }
        public void SendWithCC(string email, string subject, string content, string cc)
        {
            try
            {
                SendMail(_emailSend, _passWord, email, subject, content, cc, string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SendMail(string from, string password, string to, string subject, string htmlContent, string cc, string bcc)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = true,
                Credentials = new NetworkCredential(from, password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(from, "Admin page")
            };
            mailMessage.To.Add(to);
            if (!string.IsNullOrEmpty(cc))
            {
                mailMessage.CC.Add(cc);
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                mailMessage.Bcc.Add(bcc);
            }
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlContent;
            mailMessage.Subject = subject;
            client.Send(mailMessage);
        }
    }
}
