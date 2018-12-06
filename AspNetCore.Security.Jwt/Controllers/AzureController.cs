using AspNetCore.Security.Jwt.AzureAD;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// Azure Contoller for authentication
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [AzureAuthorizeAttribute]
    public class AzureController : Controller
    {
        private readonly IAuthentication<AzureADAuthModel, AzureADResponseModel> authentication;

        public AzureController(IAuthentication<AzureADAuthModel, AzureADResponseModel> authentication)
        {
            this.authentication = authentication;
        }

        [Route("/azure")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AzureADAuthModel user)
        {
            try
            {
                ValidateInput(user);

                var response = await this.authentication.IsValidUser(user);

                if (response.IsAuthenticated && !string.IsNullOrEmpty(response.AccessToken))
                    return new ObjectResult(response.AccessToken);

                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new SecurityException(ex.Message);
            }            
        }

        private void ValidateInput(AzureADAuthModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(user.APIKey))
            {
                throw new SecurityException($"{nameof(user.APIKey)} is null or empty.");
            }
        }
    }
}
