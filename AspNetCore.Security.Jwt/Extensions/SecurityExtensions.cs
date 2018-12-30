using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AspNetCore.Security.Jwt.UnitTests")]
namespace AspNetCore.Security.Jwt
{
    using AspNetCore.Security.Jwt.AzureAD;
    using AspNetCore.Security.Jwt.Facebook;
    using AspNetCore.Security.Jwt.Google;
    using AspNetCore.Security.Jwt.Twitter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    /// <summary>
    /// SecurityExtensions static class
    /// </summary>
    public static class SecurityExtensions
    {
        private static bool IsJwtSchemeAdded = false;
        internal static bool IsSecurityUsed { get; set; }

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
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);            
            services.AddScopedIfNotExists<ISecurityService, SecurityService>();
            services.AddScopedIfNotExists<IAuthentication, TAuthenticator>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

            services.AddSwaggerAndJwtBearerScheme(addSwaggerSecurity, securitySettings);

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
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);
            services.AddSingletonIfNotExists<BaseSecuritySettings>(securitySettings);
            if (addClaims != null)
            {
                services.AddSingletonIfNotExists<Action<IIdTypeBuilder<TUserModel>>>(x => addClaims);
            }
            services.AddScopedIfNotExists<ISecurityService<TUserModel>, SecurityService<TUserModel>>();
            services.AddScopedIfNotExists<IAuthentication<TUserModel>, TAuthenticator>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

            services.AddSwaggerAndJwtBearerScheme(addSwaggerSecurity, securitySettings);

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
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);
            services.AddSingletonIfNotExists<BaseSecuritySettings>(securitySettings);
            if (addClaims != null)
            {
                services.AddSingletonIfNotExists<Action<IIdTypeBuilder<FacebookAuthModel>>>(x => addClaims);
            }
            services.AddScopedIfNotExists<ISecurityService<FacebookAuthModel>, SecurityService<FacebookAuthModel>>();           
            services.AddScopedIfNotExists<IAuthentication<FacebookAuthModel>, FacebookAuthenticator>();
            services.AddScopedIfNotExists<ISecurityClient<FacebookAuthModel, bool>, FacebookClient>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

            services.AddSwaggerAndJwtBearerScheme(addSwaggerSecurity, securitySettings);

            return services;
        }

        /// <summary>
        /// Add Facebook security extension
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="configuration">The configurations -- appsettings</param>
        /// <param name="addSwaggerSecurity">Enable security in Swagger UI</param>
        /// <returns>The services collection</returns>
        public static IServiceCollection AddGoogleSecurity(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                bool addSwaggerSecurity = false)
        {
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);

            services.AddSingletonIfNotExists<BaseSecuritySettings>(securitySettings);
            services.AddSingletonIfNotExists<GoogleSecuritySettings>(securitySettings.GoogleSecuritySettings);
            services.AddScopedIfNotExists<IAuthentication<GoogleAuthModel, GoogleResponseModel>, GoogleAuthenticator>();
            services.AddScopedIfNotExists<ISecurityClient<GoogleAuthModel, GoogleResponseModel>, GoogleClient>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

            services.AddSwaggerAndJwtBearerScheme(addSwaggerSecurity, securitySettings);

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
        public static IServiceCollection AddTwitterSecurity(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                bool addSwaggerSecurity = false)
        {
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);

            services.AddSingletonIfNotExists<BaseSecuritySettings>(securitySettings);

            services.AddSingletonIfNotExists<TwitterSecuritySettings>(securitySettings.TwitterSecuritySettings);
            services.AddScopedIfNotExists<IAuthentication<TwitterAuthModel, TwitterResponseModel>, TwitterAuthenticator>();
            services.AddScopedIfNotExists<ISecurityClient<TwitterResponseModel>, TwitterClient>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

            services.AddSwaggerAndJwtBearerScheme(addSwaggerSecurity, securitySettings);

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
            var securitySettings = configuration.SecuritySettings();
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingletonIfNotExists(securitySettings);
            services.AddSingletonIfNotExists<AzureADSecuritySettings>(securitySettings.AzureADSecuritySettings);            
            services.AddScopedIfNotExists<IAuthentication<AzureADAuthModel, AzureADResponseModel>, AzureAuthenticator>();
            services.AddScopedIfNotExists<ISecurityClient<AzureADResponseModel>, AzureClient>();
            services.AddScopedIfNotExists<IHttpClient, HttpClientHandler>(x => new HttpClientHandler(new HttpClient()));

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
        /// Get security settings from appsettings.json
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        /// <returns><see cref="SecuritySettings"/></returns>
        internal static SecuritySettings SecuritySettings(this IConfiguration configuration)
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);

            return securitySettings;
        }

        /// <summary>
        /// Add swagger and jwt bearer scheme
        /// </summary>
        /// <param name="services">The services collection</param>
        /// <param name="addSwaggerSecurity">Add swagger security</param>
        /// <param name="securitySettings">The security settings</param>
        internal static void AddSwaggerAndJwtBearerScheme(this IServiceCollection services, bool addSwaggerSecurity, SecuritySettings securitySettings)
        {
            if (addSwaggerSecurity)
            {
                services.AddSecureSwaggerDocumentation();
            }

            if (!IsJwtSchemeAdded)
            {
                services.AddJwtBearerScheme(securitySettings);
                IsJwtSchemeAdded = true;
            }
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
            var securitySettings = settings;
            services.AddSingletonIfNotExists(securitySettings);

            services.AddSingletonIfNotExists<IAddSecurityBuilder, AddSecurityBuilder>(x => new AddSecurityBuilder(securitySettings, IsJwtSchemeAdded, services, addSwaggerSecurity));

            var sp = services.BuildServiceProvider();

            IAddSecurityBuilder addSecurityBuilder = sp.GetRequiredService<IAddSecurityBuilder>();

            return addSecurityBuilder;
        }
        
        internal static IServiceCollection AddScopedIfNotExists<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory = null)
            where TService: class
            where TImplementation: class, TService
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                if (implementationFactory == null)
                {
                    services.AddScoped<TService, TImplementation>();
                }
                else
                {
                    services.AddScoped<TService, TImplementation>(implementationFactory);
                }
            }

            return services;
        }

        internal static IServiceCollection AddSingletonIfNotExists<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                services.AddSingleton<TService, TImplementation>(implementationFactory);
            }

            return services;
        }

        internal static IServiceCollection AddSingletonIfNotExists<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory = null)
                    where TService : class
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                if (implementationFactory == null)
                {
                    services.AddSingleton<TService>();
                }
                else
                {
                    services.AddSingleton(implementationFactory);
                }
            }

            return services;
        }

        internal static IServiceCollection AddSingletonIfNotExists<TService>(this IServiceCollection services, TService implementationFactory)
                    where TService : class
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                if (implementationFactory == null)
                {
                    services.AddSingleton<TService>();
                }
                else
                {
                    services.AddSingleton(implementationFactory);
                }
            }

            return services;
        }

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app, bool addSwaggerSecurity = false)
        {
            if (!IsSecurityUsed)
            {
                if (addSwaggerSecurity)
                {
                    app.UseSwaggerDocumentation();
                }
                app.UseAuthentication();

                IsSecurityUsed = true;
            }            

            return app;
        }
    }
}
