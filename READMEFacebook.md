## 1. FacebookAuthenticator

The out of the box **FacebookAuthenticator** is automatically wired up for dependency injection.

Note:- You can inject ILogger\<FacebookAuthenticator\>, if you want logging inside the FacebookAuthenticator.

This handles the authentication of your Facebook **User Access Token**  (see Appendix A) by Facebook.


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

            services.AddFacebookSecurity(this.Configuration, builder =>
                builder.AddClaim("FacebookUser", userModel => userModel.UserAccessToken)
            , true);
            services.AddMvc().AddFacebookSecurity();
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
    "Secret": "a secret that needs to be at least 16 characters long",
    "Issuer": "your app",
    "Audience": "the client of your app",
    "IdType": "Name",
    "TokenExpiryInHours": 1.2,
    "AppId": "Your facebook app id",
    "AppSecret": "Your facebook app secret"
  },
  .
  .
  .
}
```

## FacebookController - Issues the Jwt token

The out of the box **FacebookController** issues the JWT token.

The **FacebookContoller** has a POST Method (/facebook) which you can call with a Facebook **User Access Token**  (see Appendix A).

After authentication by Facebook, a JWT bearer token is issued.

## Swagger UI integration

You will get a **Facebook endpoint (/facebook)** automatically in Swagger UI.

You will have to obtain a Facebook **User Access Token** (see Appendix A) and pass that to the Facebook endpoint.

If authenticated by Facebook, you will get a JWT bearer token back.

![Facebook Swagger UI integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/FacebookSwaggerIntegration.jpg)

## Appendix A

[**To obtain a Facebook User Access Token**](https://developers.facebook.com/docs/facebook-login/access-tokens/)