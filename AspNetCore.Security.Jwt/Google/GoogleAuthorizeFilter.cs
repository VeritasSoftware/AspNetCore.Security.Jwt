using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.Security.Jwt.Google
{
    public class GoogleAuthorizeAttributeAttribute : TypeFilterAttribute
    {
        public GoogleAuthorizeAttributeAttribute() : base(typeof(GoogleAuthorizeFilter))
        {
        }
    }

    public class GoogleAuthorizeFilter : IAuthorizationFilter
    {
        private readonly SecuritySettings securitySettings;

        public GoogleAuthorizeFilter(SecuritySettings securitySettings)
        {
            this.securitySettings = securitySettings;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var req = context.HttpContext.Request;

            // Allows using several time the stream in ASP.Net Core
            req.EnableRewind();

            using (System.IO.MemoryStream m = new System.IO.MemoryStream())
            {                    
                req.Body.CopyTo(m);

                var bodyString = System.Text.Encoding.UTF8.GetString(m.ToArray());

                context.HttpContext.Request.Body.Position = 0;

                var authModel = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleAuthModel>(bodyString);

                if ((string.Compare(authModel.APIKey?.Trim(), this.securitySettings.GoogleSecuritySettings.APIKey.Trim()) != 0)
                    ||
                    (string.IsNullOrEmpty(authModel.AuthorizationCode)))
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
