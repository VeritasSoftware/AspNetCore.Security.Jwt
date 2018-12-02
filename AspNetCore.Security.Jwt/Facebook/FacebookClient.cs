using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Facebook
{
    public class FacebookClient : ISecurityClient<FacebookAuthModel, bool>
    {
        private readonly SecuritySettings facebookSecuritySettings;

        public FacebookClient(SecuritySettings facebookSecuritySettings)
        {
            this.facebookSecuritySettings = facebookSecuritySettings;
        }

        public virtual async Task<bool> PostSecurityRequest(FacebookAuthModel user)
        {
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
    }
}
