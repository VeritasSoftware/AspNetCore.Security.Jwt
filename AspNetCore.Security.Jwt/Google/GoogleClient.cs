using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCore.Security.Jwt.Google
{
    public class GoogleClient : ISecurityClient<GoogleAuthModel, GoogleResponseModel>
    {
        private readonly GoogleSecuritySettings googleSecuritySettings;

        public GoogleClient(GoogleSecuritySettings googleSecuritySettings)
        {
            this.googleSecuritySettings = googleSecuritySettings;
        }

        public virtual async Task<GoogleResponseModel> PostSecurityRequest(GoogleAuthModel request)
        {
            string codeClient = $"code={HttpUtility.UrlEncode(request.AuthorizationCode.Trim())}&" 
                + $"client_id={HttpUtility.UrlEncode(this.googleSecuritySettings.ClientId.Trim())}&";
            string secretUri = $"client_secret={HttpUtility.UrlEncode(this.googleSecuritySettings.ClientSecret.Trim())}&" 
                + $"redirect_uri={HttpUtility.UrlEncode(this.googleSecuritySettings.RedirectUri.Trim())}&"                
                + "grant_type=authorization_code";

            var postString = codeClient + secretUri;

            byte[] bytes = Encoding.UTF8.GetBytes(postString);

            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage requestGoogle = new HttpRequestMessage(HttpMethod.Post, "https://accounts.google.com/o/oauth2/token");
                requestGoogle.Content = new ByteArrayContent(bytes);

                requestGoogle.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                requestGoogle.Content.Headers.Add("Content-Length", $"{bytes.Length}");

                var response = await httpClient.SendAsync(requestGoogle);                      

                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<GoogleResponseModel>(responseStr);

                tokenResponse.IsAuthenticated = true;

                return tokenResponse;
            }            
        }
    }
}
