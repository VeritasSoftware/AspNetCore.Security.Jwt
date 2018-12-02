using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
{
    /// <summary>
    /// FacebookAuthenticator class - Authericates the Facebook Access Token
    /// </summary>
    internal class FacebookAuthenticator : IAuthentication<FacebookAuthModel>
    {
        private readonly ISecurityClient<FacebookAuthModel, bool> securityClient;
        private readonly ILogger<FacebookAuthenticator> logger;

        public FacebookAuthenticator(ISecurityClient<FacebookAuthModel, bool> securityClient, ILogger<FacebookAuthenticator> logger = null)
        {
            this.securityClient = securityClient;
            this.logger = logger;
        }

        public async Task<bool> IsValidUser(FacebookAuthModel user)
        {
            try
            {
                ValidateInput(user);

                return await this.securityClient.PostSecurityRequest(user);
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(FacebookAuthenticator).Name}");
                }
                throw;
            }            
        }
        
        private void ValidateInput(FacebookAuthModel user)
        {
            if (this.logger != null)
            {
                logger.LogInformation($"User Access Token: {user.UserAccessToken}");
            }

            if (string.IsNullOrEmpty(user.UserAccessToken))
            {
                throw new ArgumentNullException("user.UserAccessToken");
            }
        }

    }
}
