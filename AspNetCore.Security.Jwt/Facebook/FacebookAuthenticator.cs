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
        private readonly SecuritySettings facebookSecuritySettings;
        private readonly ISecurityClient<FacebookAuthModel, bool> securityClient;
        private readonly ILogger<FacebookAuthenticator> logger;

        public FacebookAuthenticator(SecuritySettings facebookSecuritySettings, ISecurityClient<FacebookAuthModel, bool> securityClient, ILogger<FacebookAuthenticator> logger = null)
        {
            this.facebookSecuritySettings = facebookSecuritySettings;
            this.securityClient = securityClient;
            this.logger = logger;
        }

        public async Task<bool> IsValidUser(FacebookAuthModel user)
        {
            try
            {
                if (this.logger != null)
                {
                    logger.LogInformation($"User Access Token: {user.UserAccessToken}");
                }

                if (string.IsNullOrEmpty(user.UserAccessToken))
                {
                    throw new ArgumentNullException(nameof(user.UserAccessToken));
                }

                return await this.securityClient.PostSecurityRequest(user);
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(FacebookAuthenticator).Name}");
                }
                throw ex;
            }            
        }       
    }
}
