using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Twitter
{
    internal class TwitterAuthenticator : IAuthentication<TwitterAuthModel, TwitterResponseModel>
    {
        private readonly TwitterSecuritySettings twitterSecuritySettings;
        private readonly ILogger<TwitterAuthenticator> logger;
        private readonly ISecurityClient<TwitterResponseModel> securityClient;

        public TwitterAuthenticator(TwitterSecuritySettings twitterSecuritySettings, ISecurityClient<TwitterResponseModel> securityClient, ILogger<TwitterAuthenticator> logger = null)
        {
            this.twitterSecuritySettings = twitterSecuritySettings;
            this.securityClient = securityClient;
            this.logger = logger;
        }

        public async Task<TwitterResponseModel> IsValidUser(TwitterAuthModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(this.twitterSecuritySettings.APIKey) || string.IsNullOrEmpty(user.APIKey))
                {
                    return new TwitterResponseModel { IsAuthenticated = false };
                }

                if (string.Compare(user.APIKey.Trim(), this.twitterSecuritySettings.APIKey.Trim()) != 0)
                {
                    return new TwitterResponseModel { IsAuthenticated = false };
                }

                return await this.securityClient.PostSecurityRequest();
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(TwitterAuthenticator).Name}");
                }
                throw;
            }
        }
    }
}
