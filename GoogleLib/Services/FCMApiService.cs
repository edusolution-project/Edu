using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace GoogleLib.Services
{
    public class FCMApiService
    {
        private readonly GoogleCredential _googleCredential;
        public FCMApiService(GoogleCredential googleCredential)
        {
            _googleCredential = googleCredential;
        }
        public FirebaseApp Create()
        {
            return FirebaseApp.Create(new AppOptions()
            {
                Credential = _googleCredential
            });
        }
    }
}
