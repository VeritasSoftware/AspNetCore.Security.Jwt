using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
{
    public class FacebookClient : ISecurityClient<FacebookAuthModel, bool>
    {
        private readonly SecuritySettings facebookSecuritySettings;
        private readonly IHttpClient httpClient;

        public FacebookClient(SecuritySettings facebookSecuritySettings, IHttpClient httpClient)
        {
            this.facebookSecuritySettings = facebookSecuritySettings;
            this.httpClient = httpClient;
        }

        public virtual async Task<bool> PostSecurityRequest(FacebookAuthModel request)
        {
            // 1.generate an app access token
            var appAccessToken = await this.httpClient.GetStringAsync<FacebookAppAccessToken>($"https://graph.facebook.com/oauth/access_token?client_id={this.facebookSecuritySettings.AppId}&client_secret={this.facebookSecuritySettings.AppSecret}&grant_type=client_credentials");
            // 2. validate the user access token
            var userAccessTokenValidation = await this.httpClient.GetStringAsync<FacebookUserAccessTokenValidation>($"https://graph.facebook.com/debug_token?input_token={request.UserAccessToken}&access_token={appAccessToken.AccessToken}");

            return userAccessTokenValidation.Data.IsValid;
        }
    }
}
