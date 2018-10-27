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
    /// <summary>
    /// Token Contoller for default authentication
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly ISecurityService securityService;
        private readonly IAuthentication authentication;

        public TokenController(ISecurityService securityService,
                                IAuthentication authentication)
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

    /// <summary>
    /// Token Controller for custom User model
    /// </summary>
    /// <typeparam name="TUserModel">The custom User model</typeparam>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [GenericControllerNameConvention]
    public class TokenController<TUserModel> : Controller where TUserModel : class, IAuthenticationUser
    {
        private readonly ISecurityService<TUserModel> securityService;
        private readonly IAuthentication<TUserModel> authentication;

        public TokenController(ISecurityService<TUserModel> securityService,
                                IAuthentication<TUserModel> authentication)
        {
            this.securityService = securityService;
            this.authentication = authentication;
        }        

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TUserModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (await this.authentication.IsValidUser(user))
                return new ObjectResult(this.securityService.GenerateToken(user));
            return BadRequest();
        }
    }

    public class GenericTokenControllerFeatureProvider<TModel> : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(TokenController).GetTypeInfo());

            var controllerType = typeof(TokenController<>)
                        .MakeGenericType(typeof(TModel)).GetTypeInfo();
            feature.Controllers.Add(controllerType);            
        }
    }

    public class TokenControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(TokenController<>).GetTypeInfo());

            feature.Controllers.Add(typeof(TokenController).GetTypeInfo());                        
        }
    }

    public class RemoveControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {            
            if (feature.Controllers.Contains(typeof(TokenController).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(TokenController).GetTypeInfo());
            }
            if (feature.Controllers.Contains(typeof(TokenController<>).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(TokenController<>).GetTypeInfo());
            }
            if (feature.Controllers.Contains(typeof(FacebookController).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(FacebookController).GetTypeInfo());
            }
        }
    }

    public class FacebookControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public bool AddFacebookController { get; set; }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(FacebookController).GetTypeInfo());

            if (this.AddFacebookController)
            {
                feature.Controllers.Add(typeof(FacebookController).GetTypeInfo());
            }                                  
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