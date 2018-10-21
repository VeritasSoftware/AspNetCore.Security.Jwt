namespace Api.Security.Jwt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;

    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration, bool addSwaggerSecurity = true)
        {
            var securitySettings = new SecuritySettings();
            configuration.Bind("SecuritySettings", securitySettings);
            services.AddSingleton(securitySettings);
            services.AddScoped<ISecurityService, SecurityService>();

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

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app, bool addSwaggerSecurity = true)
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
