using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using AspNetCore.Security.Jwt.Google;
using AspNetCore.Security.Jwt.Twitter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

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
        private readonly bool IsInitDone = false;
        private bool IsDefaultAdded = false;
        private bool IsCustomAdded = false;
        private bool IsFacebookAdded = false;
        private bool IsGoogleAdded = false;
        private bool IsTwitterAdded = false;
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
            if (!IsInitDone)
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

                IsInitDone = true;
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
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

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
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsFacebookAdded = true;
            }            

            return this;
        }

        public IAddSecurityBuilder AddGoogleSecurity()
        {
            if (!IsGoogleAdded)
            {
                Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
                
                Services.AddSingleton<GoogleSecuritySettings>(SecuritySettings.GoogleSecuritySettings);
                Services.AddScoped<IAuthentication<GoogleAuthModel, GoogleResponseModel>, GoogleAuthenticator>();
                Services.AddScoped<ISecurityClient<GoogleAuthModel, GoogleResponseModel>, GoogleClient>();
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsGoogleAdded = true;
            }

            return this;
        }

        public IAddSecurityBuilder AddTwitterSecurity()
        {
            if (!IsTwitterAdded)
            {
                Services.AddSingleton<BaseSecuritySettings>(SecuritySettings);
                
                Services.AddSingleton<TwitterSecuritySettings>(SecuritySettings.TwitterSecuritySettings);
                Services.AddScoped<IAuthentication<TwitterAuthModel, TwitterResponseModel>, TwitterAuthenticator>();
                Services.AddScoped<ISecurityClient<TwitterResponseModel>, TwitterClient>();
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsTwitterAdded = true;
            }

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator>()
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddScoped<ISecurityService, SecurityService>();
                Services.AddScoped<IAuthentication, TAuthenticator>();
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

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
                Services.AddScoped<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsCustomAdded = true;
            }

            return this;
        }
    }
}
