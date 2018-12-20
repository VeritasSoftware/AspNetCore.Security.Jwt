namespace AspNetCore.Security.Jwt
{
    public abstract class BaseSecuritySettings
    {
        public SystemSettings SystemSettings { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double? TokenExpiryInHours { get; set; }        

        public AzureADSecuritySettings AzureADSecuritySettings { get; set; }

        public GoogleSecuritySettings GoogleSecuritySettings { get; set; }

        public TwitterSecuritySettings TwitterSecuritySettings { get; set; }
    }
        
    public class SecuritySettings : BaseSecuritySettings
    {        
        public IdType IdType { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }
    }

    public class FacebookSecuritySettings : BaseSecuritySettings
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }
    }

    public class AzureADSecuritySettings
    {
        public string AADInstance { get; set; }

        public string Tenant { get; set; }

        public string ResourceId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string APIKey { get; set; }
    }

    public class GoogleSecuritySettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }        

        public string APIKey { get; set; }
    }

    public class TwitterSecuritySettings
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string APIKey { get; set; }
    }

    public class SystemSettings
    {
        public FacebookAuthSettings FacebookAuthSettings { get; set; }

        public GoogleAuthSettings GoogleAuthSettings { get; set; }

        public TwitterAuthSettings TwitterAuthSettings { get; set; }
    }

    public class FacebookAuthSettings
    {
        public string OAuthUrl { get; set; }
        public string UserTokenValidationUrl { get; set; }
    }

    public class GoogleAuthSettings
    {
        public string TokenUrl { get; set; }
    }

    public class TwitterAuthSettings
    {
        public string TokenUrl { get; set; }
    }

}
