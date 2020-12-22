using WP = Lib.Net.Http.WebPush;

namespace WebPush.Services.Sqlite
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
}
