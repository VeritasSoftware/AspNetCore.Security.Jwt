using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Security.Jwt.Google
{
    internal class GoogleAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public GoogleAuthorizeAttributeAttribute() : base(typeof(GoogleAuthorizeFilter))
        {
        }
    }

    internal class GoogleAuthorizeFilter : BaseAuthorizeFilter<GoogleAuthModel, GoogleAuthorizeFilter>
    {
        public GoogleAuthorizeFilter(SecuritySettings securitySettings, ILogger<GoogleAuthorizeFilter> logger = null)
            : base(logger)
        {
            base.ValidCondition = authModel =>
                                    authModel != null
                                    &&
                                    (string.Compare(authModel.APIKey?.Trim(), securitySettings.GoogleSecuritySettings.APIKey.Trim()) == 0)
                                    &&
                                    (!string.IsNullOrEmpty(authModel.AuthorizationCode));
        }

    }
}
