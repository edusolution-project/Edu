﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WP = Lib.Net.Http.WebPush;

namespace WebPush.Services.Sqlite
{
    internal class PushSubscriptionContext : DbContext
    {
        public class PushSubscription : WP.PushSubscription
        {
            public string P256DH
            {
                get { return GetKey(WP.PushEncryptionKeyName.P256DH); }

                set { SetKey(WP.PushEncryptionKeyName.P256DH, value); }
            }

            public string Auth
            {
                get { return GetKey(WP.PushEncryptionKeyName.Auth); }

                set { SetKey(WP.PushEncryptionKeyName.Auth, value); }
            }

            public PushSubscription()
            { }

            public PushSubscription(WP.PushSubscription subscription)
            {
                Endpoint = subscription.Endpoint;
                Keys = subscription.Keys;
            }
        }

        public DbSet<PushSubscription> Subscriptions { get; set; }

        public PushSubscriptionContext(DbContextOptions<PushSubscriptionContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<PushSubscription> pushSubscriptionEntityTypeBuilder = modelBuilder.Entity<PushSubscription>();
            pushSubscriptionEntityTypeBuilder.HasKey(e => e.Endpoint);
            pushSubscriptionEntityTypeBuilder.Ignore(p => p.Keys);
        }
    }
}
