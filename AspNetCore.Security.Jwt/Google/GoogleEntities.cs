using Newtonsoft.Json;

namespace AspNetCore.Security.Jwt.Google
{
    public class GoogleAuthModel : IAuthenticationUser
    {
        public string AuthorizationCode { get; set; }

        public string APIKey { get; set; }
    }

    public class GoogleResponseModel : IAuthenticationUser
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
