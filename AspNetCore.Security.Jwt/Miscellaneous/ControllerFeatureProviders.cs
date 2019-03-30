using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.Security.Jwt
{
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
            if (feature.Controllers.Contains(typeof(AzureController).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(AzureController).GetTypeInfo());
            }
            if (feature.Controllers.Contains(typeof(GoogleController).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(GoogleController).GetTypeInfo());
            }
            if (feature.Controllers.Contains(typeof(TwitterController).GetTypeInfo()))
            {
                feature.Controllers.Remove(typeof(TwitterController).GetTypeInfo());
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

    public class GoogleControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public bool AddGoogleController { get; set; }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(GoogleController).GetTypeInfo());

            if (this.AddGoogleController)
            {
                feature.Controllers.Add(typeof(GoogleController).GetTypeInfo());
            }
        }
    }

    public class TwitterControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public bool AddTwitterController { get; set; }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(TwitterController).GetTypeInfo());

            if (this.AddTwitterController)
            {
                feature.Controllers.Add(typeof(TwitterController).GetTypeInfo());
            }
        }
    }

    public class AzureControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public bool AddAzureController { get; set; }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(AzureController).GetTypeInfo());

            if (this.AddAzureController)
            {
                feature.Controllers.Add(typeof(AzureController).GetTypeInfo());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
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
