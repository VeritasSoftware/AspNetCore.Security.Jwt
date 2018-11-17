using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
{
    /// <summary>
    /// FacebookAuthenticator class - Authericates the Facebook Access Token
    /// </summary>
    internal class FacebookAuthenticator : IAuthentication<FacebookAuthModel>
    {
        private readonly SecuritySettings facebookSecuritySettings;
        private readonly ILogger<FacebookAuthenticator> logger;

        public FacebookAuthenticator(SecuritySettings facebookSecuritySettings, ILogger<FacebookAuthenticator> logger = null)
        {
            this.facebookSecuritySettings = facebookSecuritySettings;
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

                using (HttpClient httpClient = new HttpClient())
                {
                    // 1.generate an app access token
                    var appAccessTokenResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={this.facebookSecuritySettings.AppId}&client_secret={this.facebookSecuritySettings.AppSecret}&grant_type=client_credentials");
                    var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
                    // 2. validate the user access token
                    var userAccessTokenValidationResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={user.UserAccessToken}&access_token={appAccessToken.AccessToken}");
                    var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

                    return userAccessTokenValidation.Data.IsValid;
                }
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
