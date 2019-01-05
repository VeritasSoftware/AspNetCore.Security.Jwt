using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Security.Jwt.Facebook
{
    internal class FacebookAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public FacebookAuthorizeAttributeAttribute() : base(typeof(FacebookAuthorizeFilter))
        {
        }
    }

    internal class FacebookAuthorizeFilter : BaseAuthorizeFilter<FacebookAuthModel, FacebookAuthorizeFilter>
    {
        public FacebookAuthorizeFilter(ILogger<FacebookAuthorizeFilter> logger = null) : base(logger)
        {
            base.ValidCondition = authModel =>
                                    authModel != null && !string.IsNullOrEmpty(authModel.UserAccessToken);
        }
    }

}
