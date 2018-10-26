using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ISecurityService securityService;
        private readonly IAuthentication authentication;

        public TokenController(ISecurityService securityService = null,
                                IAuthentication authentication = null)
        {
            this.securityService = securityService;;
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

    [Produces("application/json")]
    [Route("api/[controller]")]
    [GenericControllerNameConvention]
    public class TokenController<T> : Controller where T : class, IAuthenticationUser
    {
        private readonly ISecurityService<T> securityService;
        private readonly IAuthentication<T> authentication;

        public TokenController(ISecurityService<T> securityService,
                                IAuthentication<T> authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }        

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (await this.authentication.IsValidUser(user))
                return new ObjectResult(this.securityService.GenerateToken(user));
            return BadRequest();
        }
    }

    public class GenericControllerFeatureProvider<TModel> : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var typeName = typeof(TModel).Name + "Controller";

            var controllerType = typeof(TokenController<>)
                        .MakeGenericType(typeof(TModel)).GetTypeInfo();
            feature.Controllers.Add(controllerType);

            feature.Controllers.Remove(typeof(TokenController).GetTypeInfo());
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() !=
                typeof(TokenController<>))
            {
                // Not a GenericController, ignore.
                return;
            }

            controller.ControllerName = "Token";
        }
    }
}