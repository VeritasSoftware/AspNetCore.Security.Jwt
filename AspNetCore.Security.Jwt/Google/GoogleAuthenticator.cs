using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Google
{
    internal class GoogleAuthenticator : IAuthentication<GoogleAuthModel, GoogleResponseModel>
    {
        private readonly GoogleSecuritySettings googleSecuritySettings;
        private readonly ILogger<GoogleAuthenticator> logger;
        private readonly ISecurityClient<GoogleAuthModel, GoogleResponseModel> securityClient;

        public GoogleAuthenticator(GoogleSecuritySettings googleSecuritySettings, ISecurityClient<GoogleAuthModel, GoogleResponseModel> securityClient, ILogger<GoogleAuthenticator> logger = null)
        {
            this.googleSecuritySettings = googleSecuritySettings;
            this.securityClient = securityClient;
            this.logger = logger;
        }

        public async Task<GoogleResponseModel> IsValidUser(GoogleAuthModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(this.googleSecuritySettings.APIKey) || string.IsNullOrEmpty(user.APIKey))
                {
                    return new GoogleResponseModel { IsAuthenticated = false };
                }

                if (string.Compare(user.APIKey.Trim(), this.googleSecuritySettings.APIKey.Trim()) != 0)
                {
                    return new GoogleResponseModel { IsAuthenticated = false };
                }

                return await this.securityClient.PostSecurityRequest(user);
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(GoogleAuthenticator).Name}");
                }
                throw;
            }
        }
    }
}
