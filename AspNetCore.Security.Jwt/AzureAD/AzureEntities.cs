namespace AspNetCore.Security.Jwt.AzureAD
{
    public class AzureADAuthModel : IAuthenticationUser
    {
        public string APIKey { get; set; }
    }

    public class AzureADResponseModel : IAuthenticationUser
    {
        public string AccessToken { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
