using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// Token Contoller for default authentication
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ISecurityService securityService;
        private readonly IAuthentication authentication;

        public TokenController(ISecurityService securityService,
                                IAuthentication authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            try
            {
                ValidateInput(user);

                if (await this.authentication.IsValidUser(user.Id, user.Password))
                    return new ObjectResult(this.securityService.GenerateToken(user.Id));
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new SecurityException(ex.Message);
            }            
        }        

        private void ValidateInput(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentNullException("user.Id");
        }
    }

    /// <summary>
    /// Token Controller for custom User model
    /// </summary>
    /// <typeparam name="TUserModel">The custom User model</typeparam>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [GenericControllerNameConventionAttribute]
    public class TokenController<TUserModel> : Controller where TUserModel : class, IAuthenticationUser
    {
        private readonly ISecurityService<TUserModel> securityService;
        private readonly IAuthentication<TUserModel> authentication;

        public TokenController(ISecurityService<TUserModel> securityService,
                                IAuthentication<TUserModel> authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }        

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TUserModel user)
        {
            try
            {
                ValidateInput(user);

                if (await this.authentication.IsValidUser(user))
                    return new ObjectResult(this.securityService.GenerateToken(user));
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new SecurityException(ex.Message);
            }            
        }

        private void ValidateInput(TUserModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
        }
    }

}