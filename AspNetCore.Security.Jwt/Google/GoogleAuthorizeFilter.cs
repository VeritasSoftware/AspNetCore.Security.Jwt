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
            using (System.IO.MemoryStream m = new System.IO.MemoryStream())
            {
                if (context.HttpContext.Request.Body.CanSeek == true)
                    context.HttpContext.Request.Body.Position = 0;

                context.HttpContext.Request.Body.CopyTo(m);

                var bodyString = System.Text.Encoding.UTF8.GetString(m.ToArray());

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
