## 1. Azure Authenticator

The out of the box **AzureAuthenticator** is automatically wired up for dependency injection.

Note:- You can inject ILogger\<AzureAuthenticator\>, if you want logging inside the AzureAuthenticator.

This handles the authentication of your  **Azure Active Directory (AD) account**  by Azure AD.

Your Azure AD Security settings (Step 3) are specified in you API. These are used to automatically get a token from Azure.

It returns the token issued by Azure to the client.

## 2. In your Startup.cs

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
      "ResourceId": "https://<B2BADTenant>.onmicrosoft.com/<azureappname>",
      "ClientId": "<client-id-web-add>",
      "ClientSecret": "<client-secret>",
      "APIKey": "c1bba8a7-8a68-4697-82a6-33b4563ca895"
    }
  },
  .
  .
}
```
Replace \<B2BADTenant\>, \<azureappname\>, \<client-id-web-add\>, \<client-secret\> in the settings above from your Azure configuration.

The APIKey can be anything (like a GUID).
This is used to access the Azure endpoint of your API. It is not an Azure config item.

## 2. Accessing Azure endpoint in your API

In the Settings (Step 3), the API Key is specified for accessing the Azure endpoint.

The client has to POST to the Azure endpoint with this API Key as shown below.

```json
{
  "apiKey": "c1bba8a7-8a68-4697-82a6-33b4563ca895"
}
```

When the Azure endpoint is called, it does the Azure AD authenication and returns the JWT Bearer Token (issued by Azure) to the client.

This token can be used to access your APIs endpoints secured by Azure AD Security.

## Swagger UI integration

You will get a **Azure AD endpoint (/azure)** automatically in Swagger UI.

The client has to POST to the Azure endpoint with the API Key (Step 3).

You will be returned a JWT bearer token issued by Azure.

![Facebook Swagger UI integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/AzureADSwaggerIntegration.jpg)