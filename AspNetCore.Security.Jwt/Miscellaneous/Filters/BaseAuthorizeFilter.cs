using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    internal abstract class BaseAuthorizeFilter<TModel, TFilter> : IAsyncAuthorizationFilter
    {
        private readonly ILogger<TFilter> logger;

        protected BaseAuthorizeFilter(ILogger<TFilter> logger = null)
        {
            this.logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var req = context.HttpContext.Request;

            try
            {
                if (!await req.IsValid(this.ValidCondition))
                {
                    context.Result = new UnauthorizedResult();
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

        protected Func<TModel, bool> ValidCondition { get; set; }
    
    }
}
