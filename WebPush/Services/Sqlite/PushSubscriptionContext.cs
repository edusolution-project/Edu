using Microsoft.Extensions.Configuration;

namespace WebPush.Services.Sqlite
{
    public class PushSubscriptionContext : Core_v2.Repositories.WithoutEntityServiceBase<PushSubscription>
    {
        public PushSubscriptionContext(IConfiguration configuation) : base(configuation)
        {

        }
    }
}
