using LinkShorter.WebApi.Models;
using Microsoft.Extensions.Options;

namespace LinkShorter.WebApi.Utils
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