using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(string username, string password)
        {
            if (this.authentication.IsValidUser(username, password))
                return new ObjectResult(this.securityService.GenerateToken(username));
            return BadRequest();
            //return new ObjectResult(this.securityService.GenerateToken(username));
        }
    }
}