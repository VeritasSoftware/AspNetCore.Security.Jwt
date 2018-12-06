using AspNetCore.Security.Jwt.Google;
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
    [GoogleAuthorizeAttribute]
    public class GoogleController : Controller
    {
        private readonly IAuthentication<GoogleAuthModel, GoogleResponseModel> authentication;

        public GoogleController(IAuthentication<GoogleAuthModel, GoogleResponseModel> authentication)
        {
            this.authentication = authentication;
        }

        [Route("/google")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GoogleAuthModel user)
        {
            try
            {
                ValidateInput(user);

                var response = await this.authentication.IsValidUser(user);

                if (response.IsAuthenticated)
                    return new ObjectResult(response);
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new SecurityException(ex.Message);
            }            
        }

        private void ValidateInput(GoogleAuthModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.APIKey))
                throw new SecurityException($"{nameof(user.APIKey)} is null or empty.");
            if (string.IsNullOrEmpty(user.AuthorizationCode))
                throw new SecurityException($"{nameof(user.AuthorizationCode)} is null or empty.");            
        }
    }
}