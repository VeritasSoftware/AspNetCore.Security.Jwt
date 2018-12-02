using AspNetCore.Security.Jwt.Facebook;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// Facebook Contoller for authentication
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FacebookController : Controller
    {
        private readonly ISecurityService<FacebookAuthModel> securityService;
        private readonly IAuthentication<FacebookAuthModel> authentication;

        public FacebookController(ISecurityService<FacebookAuthModel> securityService,
                                  IAuthentication<FacebookAuthModel> authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }

        [Route("/facebook")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FacebookAuthModel user)
        {
            ValidateInput(user);

            if (await this.authentication.IsValidUser(user))
                return new ObjectResult(this.securityService.GenerateToken(user));
            return BadRequest();
        }

        private void ValidateInput(FacebookAuthModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.UserAccessToken))
                throw new SecurityException($"{nameof(user.UserAccessToken)} is null or empty.");
        }

    }
}
