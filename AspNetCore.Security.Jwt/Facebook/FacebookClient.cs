using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
{
    public class FacebookClient : ISecurityClient<FacebookAuthModel, bool>
    {
        private readonly SecuritySettings securitySettings;
        private readonly IHttpClient httpClient;

        public FacebookClient(SecuritySettings securitySettings, IHttpClient httpClient)
        {
            this.securitySettings = securitySettings;
            this.httpClient = httpClient;
        }

        public virtual async Task<bool> PostSecurityRequest(FacebookAuthModel request)
        {
            // 1.generate an app access token
            var appAccessToken = await this.httpClient.GetStringAsync<FacebookAppAccessToken>(string.Format(this.securitySettings.AuthSettings.FacebookAuthSettings.OAuthUrl, this.securitySettings.AppId, this.securitySettings.AppSecret));
            // 2. validate the user access token
            var userAccessTokenValidation = await this.httpClient.GetStringAsync<FacebookUserAccessTokenValidation>(string.Format(this.securitySettings.AuthSettings.FacebookAuthSettings.UserTokenValidationUrl, request.UserAccessToken, appAccessToken.AccessToken));

            return userAccessTokenValidation.Data.IsValid;
        }
    }
}
