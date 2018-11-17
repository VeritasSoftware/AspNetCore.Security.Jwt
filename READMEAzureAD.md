## 1. AzureAuthenticator

The out of the box **AzureAuthenticator** is automatically wired up for dependency injection.

Note:- You can inject ILogger\<AzureAuthenticator\>, if you want logging inside the AzureAuthenticator.

This handles the authentication of your  **Azure Active Directory (AD) account**  by Azure AD.

It returns the token issued by the Azure AD Security to the client.

## 2. Azure AD Auth Model

    public class AzureADAuthModel : IAuthenticationUser
    {
        public string Id { get; set; }

        public string Password { get; set; }
    }

You have to post the Azure AD User's Id and Password to the **Azure (/azure)** endpoint.'

```C#
	using AspNetCore.Security.Jwt;
	using Swashbuckle.AspNetCore.Swagger;
```

```C#
        public void ConfigureServices(IServiceCollection services)
        {
            .
            .
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "XXX API", Version = "v1" });
            });

            services.AddAzureADSecurity(this.Configuration, true);
            services.AddMvc().AddAzureADSecurity();
        }
```

```C#
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            .
            .
            .
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "XXX API V1");
            });

            app.UseSecurity(true);

            app.UseMvc();
        }
```

## 3. In your appsettings.json

**Note:-** You can put these settings in **Secret Manager** by using **Manage User Secrets** menu (right-click your Project).

```javascript
{
  "SecuritySettings": {
    .
    .
    .
    "AzureADSecuritySettings": {
      "AADInstance": "https://login.windows.net/{0}",
      "Tenant": "<B2BADTenant>.onmicrosoft.com",
      "ResourceId": "https://B2BADTenant.onmicrosoft.com/<azureappname>",
      "ClientId": "<client-id-web-add>",
      "ClientSecret": "<client-secret>"
    }
  },
}
```

When the user posts their Id and Password to the Azure endpoint, Azure AD authenication will be done and the JWT Bearer Token (issued by Azure) will be returned to the client.

This token can be used to access your APIs endpoints secured by Azure AD Security.

## Swagger UI integration

You will get a **Azure AD endpoint (/azure)** automatically in Swagger UI.

You will post our **Azure AD Id and Password** to the Azure endpoint.

If authenticated by Azure AD, you will be returned a JWT bearer token issued by Azure AD Security.

![Facebook Swagger UI integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/AzureADSwaggerIntegration.jpg)