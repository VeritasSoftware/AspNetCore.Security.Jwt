using Microsoft.AspNetCore.Mvc;
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
            if (await this.authentication.IsValidUser(user.UserName, user.Password))
                return new ObjectResult(this.securityService.GenerateToken(user.UserName));
            return BadRequest();
        }
    }
}