using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.AzureAD
{
    public class AzureClient : ISecurityClient<AzureADResponseModel>
    {
        AzureADSecuritySettings azureADSecuritySettings;

        public AzureClient(AzureADSecuritySettings azureADSecuritySettings)
        {
            this.azureADSecuritySettings = azureADSecuritySettings;
        }

        public virtual async Task<AzureADResponseModel> PostSecurityRequest()
        {
            string authority = String.Format(this.azureADSecuritySettings.AADInstance, this.azureADSecuritySettings.Tenant);

            AuthenticationContext authContext = new AuthenticationContext(authority, false);

            var response = await authContext.AcquireTokenAsync(this.azureADSecuritySettings.ResourceId, new ClientCredential(
                                                                    this.azureADSecuritySettings.ClientId, this.azureADSecuritySettings.ClientSecret));

            return new AzureADResponseModel { IsAuthenticated = true, AccessToken = response.AccessToken };
        }

    }
}
