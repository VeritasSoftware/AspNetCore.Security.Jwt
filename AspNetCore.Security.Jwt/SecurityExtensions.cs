namespace AspNetCore.Security.Jwt
{
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
                                                                Action<IIdTypeBuilder<TUserModel>> addClaims,
                                                                bool addSwaggerSecurity = false)
            where TAuthenticator : class, IAuthentication<TUserModel>
            where TUserModel : class, IAuthenticationUser
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);            
            services.AddScoped<ISecurityService<TUserModel>>(x => new SecurityService<TUserModel>(securitySettings, addClaims));
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
                                                                Action<IIdTypeBuilder<FacebookAuthModel>> addClaims,
                                                                bool addSwaggerSecurity = false)
        {
            var securitySettings = new FacebookSecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            IdTypeHelpers.LoadClaimTypes();

            services.AddSingleton(securitySettings);
            services.AddScoped<ISecurityService<FacebookAuthModel>>(x => new SecurityService<FacebookAuthModel>(securitySettings, addClaims));            
            services.AddScoped<IAuthentication<FacebookAuthModel>>(x => new FacebookAuthenticator(securitySettings));

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

        private static IServiceCollection AddJwtBearerScheme(this IServiceCollection services, BaseSecuritySettings securitySettings)
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
