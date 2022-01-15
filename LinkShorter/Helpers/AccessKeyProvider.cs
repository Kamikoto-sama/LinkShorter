using LinkShorter.Models;
using Microsoft.Extensions.Options;

namespace LinkShorter.Helpers
{
    public class AccessKeyProvider
    {
        private readonly IOptionsSnapshot<AuthSettings> appSettingsSnapshot;

        public AccessKeyProvider(IOptionsSnapshot<AuthSettings> appSettingsSnapshot)
        {
            this.appSettingsSnapshot = appSettingsSnapshot;
        }

        public bool ValidateKey(string accessKey)
        {
            return accessKey == appSettingsSnapshot.Value.AccessKey;
        }
    }
}