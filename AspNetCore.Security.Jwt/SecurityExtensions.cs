namespace AspNetCore.Security.Jwt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    public static class SecurityExtensions
    {
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
