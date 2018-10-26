using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    [Produces("application/json")]    
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ISecurityService securityService;
        private readonly IAuthentication authentication;

        public TokenController(ISecurityService securityService, IAuthentication authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentNullException(nameof(user.Id));

            if (await this.authentication.IsValidUser(user.Id, user.Password))
                return new ObjectResult(this.securityService.GenerateToken(user.Id));
            return BadRequest();
        }
    }
}