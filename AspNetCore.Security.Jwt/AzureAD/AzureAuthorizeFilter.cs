using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Security.Jwt.AzureAD
{
    internal class AzureAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public AzureAuthorizeAttributeAttribute() : base(typeof(AzureAuthorizeFilter))
        {
        }
    }

    internal class AzureAuthorizeFilter : BaseAuthorizeFilter<AzureADAuthModel, AzureAuthorizeFilter>
    {
        public AzureAuthorizeFilter(SecuritySettings securitySettings, ILogger<AzureAuthorizeFilter> logger = null)
                        : base(logger)
        {
            base.ValidCondition = authModel =>
                                    authModel != null
                                    &&
                                    (string.Compare(authModel.APIKey?.Trim(), securitySettings.AzureADSecuritySettings.APIKey.Trim()) == 0);
        }
    }
    
}
