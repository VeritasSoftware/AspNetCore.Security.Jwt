using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.AzureAD
{
    /// <summary>
    /// AzureAuthenticator class - Authericates the user with Azure AD and returns an access token
    /// </summary>
    internal class AzureAuthenticator : IAuthentication<AzureADAuthModel, AzureADResponseModel>
    {
        private readonly AzureADSecuritySettings azureSecuritySettings;
        private readonly ILogger<AzureAuthenticator> logger;
        private readonly ISecurityClient<AzureADResponseModel> securityClient;

        public AzureAuthenticator(AzureADSecuritySettings azureSecuritySettings, ISecurityClient<AzureADResponseModel> securityClient, ILogger<AzureAuthenticator> logger = null)
        {
            this.azureSecuritySettings = azureSecuritySettings;
            this.securityClient = securityClient;
            this.logger = logger;            
        }

        public async Task<AzureADResponseModel> IsValidUser(AzureADAuthModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(this.azureSecuritySettings.APIKey) || string.IsNullOrEmpty(user.APIKey))
                {
                    return new AzureADResponseModel { IsAuthenticated = false };
                }

                if (string.Compare(user.APIKey.Trim(), this.azureSecuritySettings.APIKey.Trim()) != 0)
                {
                    return new AzureADResponseModel { IsAuthenticated = false };
                }

                return await this.securityClient.PostSecurityRequest();
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(AzureAuthenticator).Name}");
                }
                throw;
            }            
        }       
    }
}
