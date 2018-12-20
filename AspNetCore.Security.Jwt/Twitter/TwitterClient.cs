using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.Twitter
{
    public class TwitterClient : ISecurityClient<TwitterResponseModel>
    {
        private readonly SecuritySettings securitySettings;
        private readonly IHttpClient httpClient;

        public TwitterClient(SecuritySettings securitySettings, IHttpClient httpClient)
        {
            this.securitySettings = securitySettings;
            this.httpClient = httpClient;
        }

        public virtual async Task<TwitterResponseModel> PostSecurityRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.securitySettings.SystemSettings.TwitterAuthSettings.TokenUrl);

            string oauth_consumer_key = this.securitySettings.TwitterSecuritySettings.ConsumerKey.Trim();
            string oauth_consumer_secret = this.securitySettings.TwitterSecuritySettings.ConsumerSecret.Trim();

            //string url = "https://api.twitter.com/oauth2/token?oauth_consumer_key=" + oauth_consumer_key + "&oauth_consumer_secret=" + oauth_consumer_secret;

            var customerInfo = Convert.ToBase64String(new UTF8Encoding()
                                .GetBytes(oauth_consumer_key + ":" + oauth_consumer_secret));

            // Add authorization to headers
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
                                                                "application/x-www-form-urlencoded");

            var response = await this.httpClient.SendAsync<TwitterResponseModel>(request);

            response.IsAuthenticated = true;

            return response;
        }
    }
}
