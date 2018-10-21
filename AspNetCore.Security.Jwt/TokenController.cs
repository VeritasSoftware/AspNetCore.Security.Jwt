using Microsoft.AspNetCore.Mvc;

namespace Api.Security.Jwt
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private readonly ISecurityService securityService;

        public TokenController(ISecurityService securityService)
        {
            this.securityService = securityService;
        }

        [Route("/token")]
        [HttpPost]
        public IActionResult Create(string username, string password)
        {
            //if (IsValidUserAndPasswordCombination(username, password))
            //    return new ObjectResult(GenerateToken(username));
            //return BadRequest();
            return new ObjectResult(this.securityService.GenerateToken(username)); ;
        }
    }
}