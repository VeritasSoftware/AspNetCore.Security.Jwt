using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AspNetCore.Security.Jwt.UnitTests")]
namespace AspNetCore.Security.Jwt
{
    using AspNetCore.Security.Jwt.AzureAD;
    using AspNetCore.Security.Jwt.Facebook;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    /// <summary>
    /// SecurityExtensions static class
    /// </summary>
    public static class SecurityExtensions
    {
        private static bool IsJwtSchemeAdded = false;
        private static SecuritySettings securitySettings;

        /// <summary>
        /// Add Security extensions - Used to wire up the dependency injection
        /// </summary>
        /// <typeparam name="TAuthenticator">The authenticator - Used to authenticate the default authentication</typeparam>
        /// <param name="services">The services collection</param>
        /// <param name="configuration">The configurations -- appsettings</param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddSecurity<TAuthenticator>(this IServiceCollection services, 
                                                                        IConfiguration configuration, 
                                                                        bool addSwaggerSecurity = false)
            where TAuthenticator : class, IAuthentication
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);            
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IAuthentication, TAuthenticator>();            

            if (addSwaggerSecurity)
            {
                services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                services.AddJwtBearerScheme(securitySettings);
                IsJwtSchemeAdded = true;
            }            

            return services;
        }

        /// <summary>
        /// Add Security extensions - Used to wire up the dependency injection
        /// </summary>
        /// <typeparam name="TAuthenticator">The authenticator - Used to authenticate the custom User model</typeparam>
        /// <typeparam name="TUserModel">The custom User model</typeparam>
        /// <param name="services">The services collection</param>
        /// <param name="configuration">The configurations -- appsettings</param>
        /// <param name="addClaims">Add the Claims using the IdTypeBuilder (<see cref="IdTypeBuilder{TUserModel}"/>)</param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddSecurity<TAuthenticator, TUserModel>(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                Action<IIdTypeBuilder<TUserModel>> addClaims = null,
                                                                bool addSwaggerSecurity = false)
            where TAuthenticator : class, IAuthentication<TUserModel>
            where TUserModel : class, IAuthenticationUser
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);
            services.AddSingleton<BaseSecuritySettings>(securitySettings);
            if (addClaims != null)
            {
                services.AddSingleton<Action<IIdTypeBuilder<TUserModel>>>(x => addClaims);
            }
            services.AddScoped<ISecurityService<TUserModel>, SecurityService<TUserModel>>();
            services.AddScoped<IAuthentication<TUserModel>, TAuthenticator>();

            if (addSwaggerSecurity)
            {
                services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                services.AddJwtBearerScheme(securitySettings);
                IsJwtSchemeAdded = true;
            }            

            return services;
        }

        /// <summary>
        /// Add Facebook security extension
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="configuration">The configurations -- appsettings</param>
        /// <param name="addClaims">Add the Claims using the IdTypeBuilder (<see cref="IdTypeBuilder{TUserModel}"/>)</param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddFacebookSecurity(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null,
                                                                bool addSwaggerSecurity = false)
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);
            services.AddSingleton<BaseSecuritySettings>(securitySettings);
            if (addClaims != null)
            {
                services.AddSingleton<Action<IIdTypeBuilder<FacebookAuthModel>>>(x => addClaims);
            }
            services.AddScoped<ISecurityService<FacebookAuthModel>, SecurityService<FacebookAuthModel>>();           
            services.AddScoped<IAuthentication<FacebookAuthModel>, FacebookAuthenticator>();

            if (addSwaggerSecurity)
            {
                services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                services.AddJwtBearerScheme(securitySettings);
                IsJwtSchemeAdded = true;
            }

            return services;
        }

        /// <summary>
        /// Add Azure AD security extension
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="configuration">The configurations -- appsettings</param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddAzureADSecurity(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                bool addSwaggerSecurity = false)
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);
            services.AddSingleton<AzureADSecuritySettings>(securitySettings.AzureADSecuritySettings);            
            services.AddScoped<IAuthentication<AzureADAuthModel, AzureADResponseModel>, AzureAuthenticator>();

            if (addSwaggerSecurity)
            {
                services.AddSecureSwaggerDocumentation();
            }            

            return services;
        } 

        internal static IServiceCollection AddJwtBearerScheme(this IServiceCollection services, BaseSecuritySettings securitySettings)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.Secret)),

                    ValidateIssuer = true,
                    ValidIssuer = securitySettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = securitySettings.Audience,

                    ValidateLifetime = true, //validate the expiration and not before values in the token

                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });

            return services;
        }

        /// <summary>
        /// Add Security extension - Invokes the AddSecurityBuilder
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="settings">The settings <see cref="SecuritySettings"/></param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns><see cref="IAddSecurityBuilder"/></returns>
        public static IAddSecurityBuilder AddSecurity(this IServiceCollection services, SecuritySettings settings, bool addSwaggerSecurity = false)
        {
            securitySettings = settings;
            services.AddSingleton(securitySettings);            

            IAddSecurityBuilder addSecurityBuilder = new AddSecurityBuilder(securitySettings, IsJwtSchemeAdded, services, addSwaggerSecurity);

            return addSecurityBuilder;
        }        

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app, bool addSwaggerSecurity = false)
        {
            if (addSwaggerSecurity)
            {
                app.UseSwaggerDocumentation();
            }            
            app.UseAuthentication();

            return app;
        }
    }
}
