using AspNetCore.Security.Jwt.Twitter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// Twitter Contoller for authentication
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [TwitterAuthorizeAttribute]
    public class TwitterController : Controller
    {
        private readonly IAuthentication<TwitterAuthModel, TwitterResponseModel> authentication;

        public TwitterController(IAuthentication<TwitterAuthModel, TwitterResponseModel> authentication)
        {
            this.authentication = authentication;
        }

        [Route("/twitter")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TwitterAuthModel user)
        {
            try
            {
                var response = await this.authentication.IsValidUser(user);

                if (response.IsAuthenticated)
                    return new ObjectResult(response.AccessToken);
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new SecurityException(ex.Message);
            }
        }

    }
}
