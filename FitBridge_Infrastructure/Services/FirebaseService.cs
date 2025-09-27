using FirebaseAdmin;
using FitBridge_Application.Configurations;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace FitBridge_Infrastructure.Services
{
    internal class FirebaseService
    {
        private readonly FirebaseApp _app;

        public FirebaseService(IOptions<FirebaseSettings> settings)
        {
            _app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(settings.Value.ServiceAccountJson)
            });
        }

        public FirebaseApp GetApp()
        {
            return _app;
        }
    }
}