using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.Security.Jwt.Facebook
{
    internal class FacebookAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public FacebookAuthorizeAttributeAttribute() : base(typeof(FacebookAuthorizeFilter))
        {
        }
    }

    internal class FacebookAuthorizeFilter : IAuthorizationFilter
    {
        private readonly SecuritySettings securitySettings;
        private readonly ILogger<FacebookAuthorizeFilter> logger;

        public FacebookAuthorizeFilter(SecuritySettings securitySettings, ILogger<FacebookAuthorizeFilter> logger = null)
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

                using (MemoryStream m = new MemoryStream())
                {
                    req.Body.CopyTo(m);

                    var bodyString = Encoding.UTF8.GetString(m.ToArray());

                    context.HttpContext.Request.Body.Position = 0;

                    var authModel = JsonConvert.DeserializeObject<FacebookAuthModel>(bodyString);

                    if (authModel == null || string.IsNullOrEmpty(authModel.UserAccessToken))
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
