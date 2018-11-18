using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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

        public AzureAuthenticator(AzureADSecuritySettings azureSecuritySettings, ILogger<AzureAuthenticator> logger = null)
        {
            this.azureSecuritySettings = azureSecuritySettings;
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

                string authority = String.Format(this.azureSecuritySettings.AADInstance, this.azureSecuritySettings.Tenant);

                AuthenticationContext authContext = new AuthenticationContext(authority, false);

                var response = await authContext.AcquireTokenAsync(this.azureSecuritySettings.ResourceId, new ClientCredential(
                                                                        this.azureSecuritySettings.ClientId, this.azureSecuritySettings.ClientSecret));

                return new AzureADResponseModel { IsAuthenticated = true, AccessToken = response.AccessToken };
            }
            catch (Exception ex)
            {
                if (this.logger != null)
                {
                    logger.LogError(ex, $"Exception in {typeof(AzureAuthenticator).Name}");
                }
                throw ex;
            }            
        }       
    }
}
