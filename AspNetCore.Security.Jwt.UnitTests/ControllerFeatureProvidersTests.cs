using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class ControllerFeatureProvidersTests
    {
        [Fact]
        public void Test_ControllerFeatureProviders_TokenControllerFeatureProvider_Pass()
        {
            var featureProvider = new TokenControllerFeatureProvider();

            var applicationParts = new List<ApplicationPart>();
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(applicationParts, controllerFeature);

            Assert.True(controllerFeature.Controllers.Any());
            Assert.True(controllerFeature.Controllers.Count == 1);
            Assert.True(controllerFeature.Controllers.First() == typeof(TokenController));
        }

        [Fact]
        public void Test_ControllerFeatureProviders_GenericTokenControllerFeatureProvider_Pass()
        {
            var featureProvider = new GenericTokenControllerFeatureProvider<UserModel>();

            var applicationParts = new List<ApplicationPart>();
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(applicationParts, controllerFeature);

            Assert.True(controllerFeature.Controllers.Any());
            Assert.True(controllerFeature.Controllers.Count == 1);
            Assert.True(controllerFeature.Controllers.First() == typeof(TokenController<UserModel>));
        }

        [Fact]
        public void Test_ControllerFeatureProviders_AzureADControllerFeatureProvider_Pass()
        {
            var featureProvider = new AzureControllerFeatureProvider()
            {
                AddAzureController = true
            };

            var applicationParts = new List<ApplicationPart>();
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(applicationParts, controllerFeature);

            Assert.True(controllerFeature.Controllers.Any());
            Assert.True(controllerFeature.Controllers.Count == 1);
            Assert.True(controllerFeature.Controllers.First() == typeof(AzureController));
        }

        [Fact]
        public void Test_ControllerFeatureProviders_FacebookControllerFeatureProvider_Pass()
        {
            var featureProvider = new FacebookControllerFeatureProvider()
            {
                AddFacebookController = true
            };

            var applicationParts = new List<ApplicationPart>();
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(applicationParts, controllerFeature);

            Assert.True(controllerFeature.Controllers.Any());
            Assert.True(controllerFeature.Controllers.Count == 1);
            Assert.True(controllerFeature.Controllers.First() == typeof(FacebookController));
        }

        [Fact]
        public void Test_ControllerFeatureProviders_RemoveControllerFeatureProvider_Pass()
        {
            var featureProvider = new RemoveControllerFeatureProvider();

            var applicationParts = new List<ApplicationPart>();
            var controllerFeature = new ControllerFeature();

            controllerFeature.Controllers.Add(typeof(TokenController).GetTypeInfo());
            controllerFeature.Controllers.Add(typeof(TokenController<>).GetTypeInfo());
            controllerFeature.Controllers.Add(typeof(AzureController).GetTypeInfo());
            controllerFeature.Controllers.Add(typeof(FacebookController).GetTypeInfo());
            
            featureProvider.PopulateFeature(applicationParts, controllerFeature);

            Assert.True(!controllerFeature.Controllers.Any());
        }

    }
}
