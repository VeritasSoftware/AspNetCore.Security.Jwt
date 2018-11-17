namespace AspNetCore.Security.Jwt.AzureAD
{
    public class AzureADAuthModel : IAuthenticationUser
    {
        public string Id { get; set; }

        public string Password { get; set; }
    }

    public class AzureADResponseModel : IAuthenticationUser
    {
        public string AccessToken { get; set; }
    }
}
