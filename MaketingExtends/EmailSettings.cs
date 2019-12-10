using System;

namespace MaketingExtends
{
    //"EmailSettings": {
    //    "MailServer": "smtp.some_server.com",
    //    "MailPort": 587,
    //    "SenderName": "some name",
    //    "Sender": "some_email@some_server.com",
    //    "Password": "some_password"
    //}
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
    }
}
