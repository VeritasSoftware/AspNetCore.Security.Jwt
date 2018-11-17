﻿using AspNetCore.Security.Jwt.AzureAD;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
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
                if (this.logger != null)
                {
                    logger.LogInformation($"Azure AD User Id: {user.Id}");
                }
                
                string authority = String.Format(this.azureSecuritySettings.AADInstance, this.azureSecuritySettings.Tenant);

                AuthenticationContext authContext = new AuthenticationContext(authority);

                var response = await authContext.AcquireTokenAsync(this.azureSecuritySettings.ResourceId, new ClientCredential(
                                                                        this.azureSecuritySettings.ClientId, this.azureSecuritySettings.ClientSecret));

                return new AzureADResponseModel { AccessToken = response.AccessToken };
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
