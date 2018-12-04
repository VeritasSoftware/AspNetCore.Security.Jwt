using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using AspNetCore.Security.Jwt.Google;
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

        private readonly SecuritySettings SecuritySettings;
        private bool IsDefaultAdded = false;
        private bool IsCustomAdded = false;
        private bool IsFacebookAdded = false;
        private bool IsGoogleAdded = false;
        private bool IsAzureAdded = false;
        private readonly bool IsSwaggerAdded = false;
        private readonly IServiceCollection Services;

        public static AddSecurityBuilder TheInstance()
        {
            lock (padlock)
            {                  
                return instance;
            }                           
        }

        private AddSecurityBuilder(SecuritySettings securitySettings, bool isJwtSchemeAdded, IServiceCollection services, bool addSwaggerSecurity = false)
        {
            if (!IsDefaultAdded || !IsCustomAdded || !IsAzureAdded || !IsFacebookAdded)
            {
                SecuritySettings = securitySettings;
                Services = services;

                IdTypeHelpers.LoadClaimTypes();

                if (!isJwtSchemeAdded)
                {
                    Services.AddJwtBearerScheme(SecuritySettings);
                }

                if (addSwaggerSecurity && !IsSwaggerAdded)
                {
                    Services.AddSecureSwaggerDocumentation();

                    IsSwaggerAdded = true;
                }
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
                Services.AddScoped<IHttpClient, HttpClientHandler>();

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
                Services.AddScoped<ISecurityClient<FacebookAuthModel, bool>, FacebookClient>();
                Services.AddScoped<IHttpClient, HttpClientHandler>();

                IsFacebookAdded = true;
            }            

            return this;
        }

        public IAddSecurityBuilder AddGoogleSecurity(Action<IIdTypeBuilder<GoogleAuthModel>> addClaims = null)
        {
            if (!IsGoogleAdded)
            {
                Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
                if (addClaims != null)
                {
                    Services.AddSingleton<Action<IIdTypeBuilder<GoogleAuthModel>>>(x => addClaims);
                }
                Services.AddSingleton<GoogleSecuritySettings>(SecuritySettings.GoogleSecuritySettings);
                Services.AddScoped<IAuthentication<GoogleAuthModel, GoogleResponseModel>, GoogleAuthenticator>();
                Services.AddScoped<ISecurityClient<GoogleAuthModel, GoogleResponseModel>, GoogleClient>();
                Services.AddScoped<IHttpClient, HttpClientHandler>();

                IsGoogleAdded = true;
            }

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator>()
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddScoped<ISecurityService, SecurityService>();
                Services.AddScoped<IAuthentication, TAuthenticator>();
                Services.AddScoped<IHttpClient, HttpClientHandler>();

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
                Services.AddScoped<IHttpClient, HttpClientHandler>();

                IsCustomAdded = true;
            }

            return this;
        }
    }
}
