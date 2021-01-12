using Lib.Net.Http.WebPush;
using System.Net.Http;

namespace WebPush.Models
{
    public class PushMessageEntity : PushMessage
    {
        public string UserId { get; set; }
        public PushMessageEntity(string content) : base(content)
        {

        }
        public PushMessageEntity(HttpContent content) : base(content)
        {

        }
    }
}
