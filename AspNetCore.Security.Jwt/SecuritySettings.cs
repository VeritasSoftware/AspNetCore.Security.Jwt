namespace AspNetCore.Security.Jwt
{
    public abstract class BaseSecuritySettings
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double? TokenExpiryInHours { get; set; }
    }
        
    public class SecuritySettings : BaseSecuritySettings
    {        
        public IdType IdType { get; set; }
    }

    public class FacebookSecuritySettings : BaseSecuritySettings
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }
    }
}
