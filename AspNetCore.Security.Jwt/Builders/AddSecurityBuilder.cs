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

        public AddSecurityBuilder(SecuritySettings securitySettings, bool isJwtSchemeAdded, IServiceCollection services, bool addSwaggerSecurity = false)
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

        public IAddSecurityBuilder AddAzureADSecurity()
        {
            if (!IsAzureAdded)
            {                
                Services.AddSingletonIfNotExists<AzureADSecuritySettings>(SecuritySettings.AzureADSecuritySettings);
                Services.AddScopedIfNotExists<IAuthentication<AzureADAuthModel, AzureADResponseModel>, AzureAuthenticator>();
                Services.AddScopedIfNotExists<ISecurityClient<AzureADResponseModel>, AzureClient>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsAzureAdded = true;
            }            

            return this;
        }

        public IAddSecurityBuilder AddFacebookSecurity(Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null)
        {
            if (!IsFacebookAdded)
            {
                Services.AddSingletonIfNotExists<BaseSecuritySettings>(SecuritySettings);
                if (addClaims != null)
                {
                    Services.AddSingletonIfNotExists<Action<IIdTypeBuilder<FacebookAuthModel>>>(x => addClaims);
                }
                Services.AddScopedIfNotExists<ISecurityService<FacebookAuthModel>, SecurityService<FacebookAuthModel>>();
                Services.AddScopedIfNotExists<IAuthentication<FacebookAuthModel>, FacebookAuthenticator>();
                Services.AddScopedIfNotExists<ISecurityClient<FacebookAuthModel, bool>, FacebookClient>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsFacebookAdded = true;
            }            

            return this;
        }

        public IAddSecurityBuilder AddGoogleSecurity()
        {
            if (!IsGoogleAdded)
            {
                Services.AddSingletonIfNotExists<BaseSecuritySettings>(SecuritySettings);
                
                Services.AddSingletonIfNotExists<GoogleSecuritySettings>(SecuritySettings.GoogleSecuritySettings);
                Services.AddScopedIfNotExists<IAuthentication<GoogleAuthModel, GoogleResponseModel>, GoogleAuthenticator>();
                Services.AddScopedIfNotExists<ISecurityClient<GoogleAuthModel, GoogleResponseModel>, GoogleClient>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsGoogleAdded = true;
            }

            return this;
        }

        public IAddSecurityBuilder AddTwitterSecurity()
        {
            if (!IsTwitterAdded)
            {
                Services.AddSingletonIfNotExists<BaseSecuritySettings>(SecuritySettings);
                
                Services.AddSingletonIfNotExists<TwitterSecuritySettings>(SecuritySettings.TwitterSecuritySettings);
                Services.AddScopedIfNotExists<IAuthentication<TwitterAuthModel, TwitterResponseModel>, TwitterAuthenticator>();
                Services.AddScopedIfNotExists<ISecurityClient<TwitterResponseModel>, TwitterClient>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsTwitterAdded = true;
            }

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator>(Action<IIdTypeBuilder> addClaims)
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddScopedIfNotExists<ISecurityService, SecurityService>();
                if (addClaims != null)
                {
                    Services.AddSingletonIfNotExists<Action<IIdTypeBuilder>>(x => addClaims);
                }
                Services.AddScopedIfNotExists<IAuthentication, TAuthenticator>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsDefaultAdded = true;
            }            

            return this;
        }

        IAddSecurityBuilder IAddSecurityBuilder.AddSecurity<TAuthenticator, TUserModel>(Action<IIdTypeBuilder<TUserModel>> addClaims)
        {
            if (!IsDefaultAdded && !IsCustomAdded)
            {
                Services.AddSingletonIfNotExists<BaseSecuritySettings>(SecuritySettings);
                if (addClaims != null)
                {
                    Services.AddSingletonIfNotExists<Action<IIdTypeBuilder<TUserModel>>>(x => addClaims);
                }
                Services.AddScopedIfNotExists<ISecurityService<TUserModel>, SecurityService<TUserModel>>();
                Services.AddScopedIfNotExists<IAuthentication<TUserModel>, TAuthenticator>();
                Services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

                IsCustomAdded = true;
            }

            return this;
        }
    }
}
