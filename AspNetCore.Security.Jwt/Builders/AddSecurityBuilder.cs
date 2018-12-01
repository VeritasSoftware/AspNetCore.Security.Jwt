using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Security.Jwt
{
    /// <summary>
    /// AddSecurityBuilder singleton class
    /// </summary>
    internal class AddSecurityBuilder : IAddSecurityBuilder
    {
        private static AddSecurityBuilder instance = null;
        private static readonly object padlock = new object();

        private SecuritySettings SecuritySettings;
        private bool IsJwtSchemeAdded = false;
        private bool IsDefaultAdded = false;
        private bool IsCustomAdded = false;
        private bool IsFacebookAdded = false;
        private bool IsAzureAdded = false;
        private bool IsSwaggerAdded = false;
        private IServiceCollection Services;

        public static AddSecurityBuilder Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        throw new Exception("AddSecurityBuilder instance not created");
                    }
                    return instance;
                }
            }
        }

        private AddSecurityBuilder(SecuritySettings securitySettings, bool isJwtSchemeAdded, IServiceCollection services, bool addSwaggerSecurity = false)
        {
            IsJwtSchemeAdded = false;
            IsAzureAdded = false;
            IsCustomAdded = false;
            IsDefaultAdded = false;
            IsSwaggerAdded = false;
            IsFacebookAdded = false;

            SecuritySettings = securitySettings;
            IsJwtSchemeAdded = isJwtSchemeAdded;
            Services = services;

            IdTypeHelpers.LoadClaimTypes();

            if (!IsJwtSchemeAdded)
            {
                Services.AddJwtBearerScheme(SecuritySettings);
                IsJwtSchemeAdded = true;
            }

            if (addSwaggerSecurity && !IsSwaggerAdded)
            {
                Services.AddSecureSwaggerDocumentation();

                IsSwaggerAdded = true;
            }
        }

        public static void Create(SecuritySettings securitySettings, bool isJwtSchemeAdded, IServiceCollection services, bool addSwaggerSecurity = false)
        {
            instance = new AddSecurityBuilder(securitySettings, isJwtSchemeAdded, services, addSwaggerSecurity);
        }

        public IAddSecurityBuilder AddAzureADSecurity()
        {
            if (!IsAzureAdded)
            {                
                Services.AddSingleton<AzureADSecuritySettings>(SecuritySettings.AzureADSecuritySettings);
                Services.AddScoped<IAuthentication<AzureADAuthModel, AzureADResponseModel>, AzureAuthenticator>();
                Services.AddScoped<ISecurityClient<AzureADResponseModel>, AzureClient>();

                IsAzureAdded = true;
            }            

            return this;
        }

        public IAddSecurityBuilder AddFacebookSecurity(Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null)
        {
            if (!IsFacebookAdded)
            {
                Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
                if (addClaims != null)
                {
                    Services.AddSingleton<Action<IIdTypeBuilder<FacebookAuthModel>>>(x => addClaims);
                }
                Services.AddScoped<ISecurityService<FacebookAuthModel>, SecurityService<FacebookAuthModel>>();
                Services.AddScoped<IAuthentication<FacebookAuthModel>, FacebookAuthenticator>();                             

                IsFacebookAdded = true;
            }            

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator>()
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddScoped<ISecurityService, SecurityService>();
                Services.AddScoped<IAuthentication, TAuthenticator>();                

                IsDefaultAdded = true;
            }            

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator, TUserModel>(Action<IIdTypeBuilder<TUserModel>> addClaims)
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
                if (addClaims != null)
                {
                    Services.AddSingleton<Action<IIdTypeBuilder<TUserModel>>>(x => addClaims);
                }
                Services.AddScoped<ISecurityService<TUserModel>, SecurityService<TUserModel>>();
                Services.AddScoped<IAuthentication<TUserModel>, TAuthenticator>();                

                IsCustomAdded = true;
            }

            return this;
        }
    }
}
