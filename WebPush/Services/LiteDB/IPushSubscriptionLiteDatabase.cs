using System.Collections.Generic;
using Lib.Net.Http.WebPush;

namespace WebPush.Services.LiteDB
{
    internal interface IPushSubscriptionLiteDatabase
    {
        void Add(PushSubscription subscription);

        void Remove(string endpoint);

        IEnumerable<PushSubscription> GetAll();
    }
}
