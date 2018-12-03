namespace AspNetCore.Security.Jwt
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
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
                    // Swagger 2.+ support
                    var security = new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", new string[] { }},
                    };

                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    c.AddSecurityRequirement(security);
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
