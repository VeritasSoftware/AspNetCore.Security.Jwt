using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    [Produces("application/json")]
    [Route("api/Token")]
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
        public async Task<IActionResult> Create(string username, string password)
        {
            if (await this.authentication.IsValidUser(username, password))
                return new ObjectResult(this.securityService.GenerateToken(username));
            return BadRequest();
        }
    }
}