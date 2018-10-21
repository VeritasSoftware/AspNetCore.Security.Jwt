namespace AspNetCore.Security.Jwt
{
    public class SecuritySettings
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int? TokenExpiryInHours { get; set; }

        public IdType IdType { get; set; }
    }
}
