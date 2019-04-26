namespace AspNetCore.Security.Jwt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerUI;
    using System.Collections.Generic;

    public static class SwaggerExtensions
    {
        internal static bool IsSwaggerAdded { get; set; }
        internal static bool IsSwaggerSecurityUsed { get; set; }

        public static IServiceCollection AddSecureSwaggerDocumentation(this IServiceCollection services)
        {
            if (!IsSwaggerAdded)
            {                  
                services.AddSwaggerGen(c =>
                {

                    // Swagger support                    
                    var scheme = new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        BearerFormat = "JWT",
                        Scheme = "bearer"                        
                    };                    

                    c.AddSecurityDefinition("Bearer", scheme);

                    var requirement = new OpenApiSecurityRequirement();
                    requirement.Add(new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, new List<string>());

                    c.AddSecurityRequirement(requirement);
                });

                IsSwaggerAdded = true;
            }            

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            if (!IsSwaggerSecurityUsed)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DocExpansion(DocExpansion.None);
                });

                IsSwaggerSecurityUsed = true;
            }            

            return app;
        }
    }

}
