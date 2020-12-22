using Lib.Net.Http.WebPush;

namespace WebPush.Models
{
    public class PushMessageViewModel
    {
        public string Content { get; set; }
        public string Topic { get; set; }

        public string Notification { get; set; }

        public PushMessageUrgency Urgency { get; set; } = PushMessageUrgency.Normal;
    }
}
