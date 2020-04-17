﻿using BaseCustomerEntity.Database;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class MailHelper
    {
        private readonly MailLogService _mailLogService;

        private IConfiguration _configuration;
        private string _defaultSender;
        private string _defaultPassword;

        public MailHelper(IConfiguration iConfig,
            MailLogService mailLogService
        )
        {
            _configuration = iConfig;
            _mailLogService = mailLogService;
            _defaultSender = _configuration.GetValue<string>("MailConfig:Email");
            _defaultPassword = _configuration.GetValue<string>("MailConfig:Password");
        }

        private static readonly string legalFooter = "<div style='padding-top:20px; border-top:solid 1px #CCC; margin-top: 50px; font-size: 0.6rem; color: #999'>" +
               "<p>Email này cùng các tệp tin đính kèm là các thông tin bảo mật của Eduso và chỉ có mục đích được gửi cho những người nhận được nêu tại email. Nếu Quý vị không phải là người nhận dự kiến của email này cùng các tập tin kèm theo, vui lòng không thực hiện bất cứ hành động nào trên cơ sở email và các tập tin này. Việc chia sẻ, phát tán bất cứ nội dung nào của email này cùng các tập tin đính kèm là hoàn toàn không được phép nếu không có sự đồng ý bằng văn bản của Eduso. Eduso không chịu trách nhiệm về sự truyền tải chính xác, đầy đủ và kịp thời của thông tin trong email và các tập tin này. Trường hợp Quý vị nhận được email này do có sự nhầm lẫn hoặc lỗi hệ thống, vui lòng thông báo cho Eduso qua email này và xóa email cùng các tập tin đính kèm khỏi hệ thống của Quý vị.Trân trọng cảm ơn.</p>" +
               "</div>";


        public async Task<int> SendBaseEmail(List<string> toAddresses, string subject, string body, int action_type, List<string> ccAddresses = null, List<string> bccAddressses = null, string fromMail = "", string fromPass = "", string fromName = "", List<AFile> files = null)
        {
            string senderID = _defaultSender;
            if (!String.IsNullOrEmpty(fromMail)) senderID = fromMail;
            //string senderPassword = ConfigurationManager.AppSettings["MailPassword"]; // sender password here…
            string senderPassword = _defaultPassword;
            if (!String.IsNullOrEmpty(fromPass)) senderPassword = fromPass;
            var senderName = "CSKH Eduso";
            if (!String.IsNullOrEmpty(fromName)) senderName = fromName;
            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                    Timeout = 30000,
                };

                var fromAddress = new MailAddress(senderID, senderName);

                MailMessage message = new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body + legalFooter,
                    IsBodyHtml = true,
                };

                foreach (var address in toAddresses)
                    message.To.Add(address);
                //message.To.Add("vietphung.it@gmail.com");
                if (ccAddresses != null && ccAddresses.Count > 0)
                    foreach (var ccAddress in ccAddresses)
                    {
                        message.CC.Add(ccAddress);
                    }
                else
                    message.CC.Add(senderID);
                if (bccAddressses != null)
                    foreach (var bccAddress in bccAddressses)
                    {
                        message.Bcc.Add(bccAddress);
                    }

                if (files != null)
                    foreach (var t in files)
                        AddAttachment(message, t);
                smtp.Send(message);
                var maillog = new MailLogEntity
                {
                    ActionType = action_type,
                    SendTime = DateTime.Now,
                    Sender = senderID,
                    Receiver = string.Join("; ", toAddresses),
                    Type = toAddresses.Count > 1 ? MailType.BULK : MailType.INDIVIDUAL,
                };
                _mailLogService.Save(maillog);
                return ResultState.OK;
            }
            catch (Exception ex)
            {
                return ResultState.ERR;
            }
        }

        public static void AddAttachment(MailMessage message, AFile file)
        {
            Attachment attachment = new Attachment(file.path, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(file.path);
            disposition.ModificationDate = File.GetLastWriteTime(file.path);
            disposition.ReadDate = File.GetLastAccessTime(file.path);
            disposition.FileName = file.filename;
            disposition.Size = new FileInfo(file.path).Length;
            disposition.DispositionType = DispositionTypeNames.Attachment;
            message.Attachments.Add(attachment);
        }

        public async Task SendRegisterEmail(AccountEntity user, string Password)
        {
            string subject = "Chúc mừng " + user.Name + " đã đăng ký tài khoản thành công tại Eduso";
            string body = "Chào " + user.Name + "," +
                "<p>Tài khoản của bạn đã khởi tạo thành công trên nền tảng hỗ trợ học tập của <a href='https://eduso.vn'>Eduso</a></p>" +
                "<p>Thông tin đăng nhập như sau</p>" +
                "<p>Tên đăng nhập: <b>" + user.UserName + "</b></p>" +
                "<p>Mật khẩu: <b>" + Password + "</b></p>" +
                "<p>Đăng nhập để trải nghiệm ngay hàng trăm khóa học hấp dẫn & hữu ích nhé.<p>" +
                "<p>Đến ngay <a href='https://eduso.vn'>Eduso.vn</a><p>";
            var toAddress = new List<string> { user.UserName };
            _ = await SendBaseEmail(toAddress, subject, body, MailPhase.REGISTER, bccAddressses: new List<string> { _defaultSender });
        }

        public async Task SendResetPass(string userID)
        {

        }
    }

    public class AFile
    {
        public string filename { get; set; }
        public string extension { get; set; }
        public string path { get; set; }
    }

    public class MailPhase
    {
        public const int REGISTER = 1, RESET_PASS = 2, JOIN_CLASS = 3, LEAVE_CLASS = 4, WEEKLY_SCHEDULE = 5;
    }

    public class ResultState
    {
        public const int OK = 1, ERR = 0;
    }

}
