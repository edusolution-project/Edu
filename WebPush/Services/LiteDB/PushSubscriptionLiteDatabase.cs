using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using LiteDB;
using WP = Lib.Net.Http.WebPush;

namespace WebPush.Services.LiteDB
{
    internal class PushSubscriptionLiteDatabase : IPushSubscriptionLiteDatabase, IDisposable
    {
        private class PushSubscription : WP.PushSubscription
        {
            public int Id { get; set; }

            public PushSubscription()
            { }

            public PushSubscription(WP.PushSubscription subscription)
            {
                Endpoint = subscription.Endpoint;
                Keys = subscription.Keys;
            }
        }

        private const string LITEDB_CONNECTION_STRING_NAME = "PushSubscriptionLiteDBDatabase";
        private const string SUBSCRIPTIONS_COLLECTION_NAME = "subscriptions";

        private readonly LiteDatabase _liteDatabase;
        private readonly LiteCollection<PushSubscription> _subscriptions;

        public PushSubscriptionLiteDatabase(IConfiguration configuration)
        {
            _liteDatabase = new LiteDatabase(configuration.GetConnectionString(LITEDB_CONNECTION_STRING_NAME));
            _subscriptions = (LiteCollection<PushSubscription>)_liteDatabase.GetCollection<PushSubscription>(SUBSCRIPTIONS_COLLECTION_NAME);
        }

        public void Add(WP.PushSubscription subscription)
        {
            _subscriptions.Insert(new PushSubscription(subscription));
        }

        public void Remove(string endpoint)
        {
            _subscriptions.Delete(subscription => subscription.Endpoint == endpoint);
        }

        public IEnumerable<WP.PushSubscription> GetAll()
        {
            return _subscriptions.FindAll();
        }

        public void Dispose()
        {
            _liteDatabase.Dispose();
        }
    }
}
