using Newtonsoft.Json;

namespace AspNetCore.Security.Jwt.Twitter
{
    public class TwitterAuthModel : IAuthenticationUser
    {
        public string APIKey { get; set; }
    }

    public class TwitterResponseModel
    {
        public bool IsAuthenticated { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
