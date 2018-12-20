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

            var securitySettings = new SecuritySettings();
            this.Configuration.Bind("SecuritySettings", securitySettings);

            services.AddSecurity(securitySettings, true)
                    .AddTwitterSecurity();            

            services.AddMvc().AddTwitterSecurity();
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

```javascript
{
  "SecuritySettings": {
    "SystemSettings": {
      .
      .
      "TwitterAuthSettings": {
        "TokenUrl": "https://api.twitter.com/oauth2/token"
      }
    },
    .
    .
    .
    "TwitterSecuritySettings": {
       "ConsumerKey": "<consumer key>",
       "ConsumerSecret": "<consumer secret>",
       "APIKey": "Your Twitter endpoint access key"
    }
  }
  .
  .
  .
}
```
Add your Twitter security settings. 

Do not change the **SystemSettings**.

The **TwitterSecuritySettings** are configurable.

## TwitterController

The out of the box **TwitterController** passes the Twitter access token to the client.

The **TwitterContoller** has a POST Method (/twitter) which you can call with the **API Key**.
And an API Key for the endpoint, specified in TwitterSecuritySettings earlier.

The access token issued by Twitter is returned to the client.

Sample request to /twitter endpoint:

```javascript
{
  "apiKey": "c1bba8a7-8a68-4697-82a6-33b4563ca895"
}
```

Sample response:

```javascript
"AAAAAAAAAAAAAAAAAAAAAKYxUgAAAAAAvt5RRnHfJOrJa0aFQxt1iyZjQgs%3DtmlrfKDW602zOUNchylCZ9k2oJbkUnIL0hzsA2Tr8qPICj1hG6"
```

## Swagger UI integration

You will get a **Twitter endpoint (/twitter)** automatically in Swagger UI.

You can POST to this endpoint with a Twitter endpoint API Key.

If authenticated by Twitter, you will get an Twitter access token back.

![Twitter Swagger UI integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/TwitterSwaggerIntegration.jpg)