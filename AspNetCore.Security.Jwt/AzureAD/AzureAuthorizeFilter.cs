using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace AspNetCore.Security.Jwt.AzureAD
{
    internal class AzureAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public AzureAuthorizeAttributeAttribute() : base(typeof(AzureAuthorizeFilter))
        {
        }
    }

    internal class AzureAuthorizeFilter : IAuthorizationFilter
    {
        private readonly SecuritySettings securitySettings;
        private readonly ILogger<AzureAuthorizeFilter> logger;

        public AzureAuthorizeFilter(SecuritySettings securitySettings, ILogger<AzureAuthorizeFilter> logger = null)
        {
            this.securitySettings = securitySettings;
            this.logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var req = context.HttpContext.Request;

            try
            {
                // Allows using several time the stream in ASP.Net Core
                req.EnableRewind();

                using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                {
                    req.Body.CopyTo(m);

                    var bodyString = System.Text.Encoding.UTF8.GetString(m.ToArray());

                    context.HttpContext.Request.Body.Position = 0;

                    var authModel = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureADAuthModel>(bodyString);

                    if (authModel == null
                        ||
                        (string.Compare(authModel.APIKey?.Trim(), this.securitySettings.AzureADSecuritySettings.APIKey.Trim()) != 0))
                    {
                        context.Result = new UnauthorizedResult();
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError(ex, "Error in Authorization. Please try again.");
                }

                context.Result = new UnauthorizedResult();

                throw new SecurityException(ex.Message);
            }
        }
    }
}
