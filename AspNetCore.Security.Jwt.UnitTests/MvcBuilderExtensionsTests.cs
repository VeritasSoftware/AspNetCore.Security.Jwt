using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class MvcBuilderExtensionsTests
    {
        [Fact]
        public void Test_MvcBuilderExtensions_Default_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMvc().AddSecurity();

            //Act
            var sp = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.True(MvcBuilderExtensions.IsDefaultSecurityAdded);
        }

        [Fact]
        public void Test_MvcBuilderExtensions_CustomUserModel_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMvc().AddSecurity<UserModel>();

            //Act
            var sp = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.True(MvcBuilderExtensions.IsUserModelSecurityAdded);
        }

        [Fact]
        public void Test_MvcBuilderExtensions_AzureAD_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMvc().AddAzureADSecurity();

            //Act
            var sp = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.True(MvcBuilderExtensions.IsAzureSecurityAdded);
        }

        [Fact]
        public void Test_MvcBuilderExtensions_Facebook_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMvc().AddFacebookSecurity();

            //Act
            var sp = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.True(MvcBuilderExtensions.IsFacebookSecurityAdded);
        }

        [Fact]
        public void Test_MvcBuilderExtensions_Multiple_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMvc()
                             .AddSecurity<UserModel>()
                             .AddAzureADSecurity()
                             .AddFacebookSecurity();

            //Act
            var sp = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.True(MvcBuilderExtensions.IsUserModelSecurityAdded);
            Assert.True(MvcBuilderExtensions.IsAzureSecurityAdded);
            Assert.True(MvcBuilderExtensions.IsFacebookSecurityAdded);
        }

    }
}
