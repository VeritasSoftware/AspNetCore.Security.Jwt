using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Security.Jwt.Twitter
{
    internal class TwitterAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public TwitterAuthorizeAttributeAttribute() : base(typeof(TwitterAuthorizeFilter))
        {
        }
    }

    internal class TwitterAuthorizeFilter : BaseAuthorizeFilter<TwitterAuthModel, TwitterAuthorizeFilter>
    {
        public TwitterAuthorizeFilter(SecuritySettings securitySettings, ILogger<TwitterAuthorizeFilter> logger = null)
            : base (logger)
        {
            base.ValidCondition = authModel => 
                                    authModel != null
                                    &&
                                    (string.Compare(authModel.APIKey?.Trim(), securitySettings.TwitterSecuritySettings.APIKey.Trim()) == 0);

        }
    }

}
