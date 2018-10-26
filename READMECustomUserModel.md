## Create the User Model

Your custom User model must inherit from **IAuthenticationUser**.

```C#
    using AspNetCore.Security.Jwt;
    using System;
    .
    .
    public class UserModel : IAuthenticationUser
    {
        public string Id { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public DateTime DOB { get; set; }
    }
```
## 1. Implement IAuthentication\<TUserModel\> interface in your app

Validate your User model here.

After this validation, the Jwt token is issued by the **TokenController**.

```C#
	using AspNetCore.Security.Jwt;
	using System.Threading.Tasks;

	namespace XXX.API
	{
		public class Authenticator : IAuthentication<UserModel>
		{        
			public async Task<bool> IsValidUser(UserModel user)
			{
				//Put your User model authenication here.
				return true;
			}
		}
	}
```

The Authenticator is automatically wired up for dependency injection (Scoped).

## 2. In your Startup.cs

Specify your **IdTypes** (**ClaimTypes**) from your custom User model using **AddIdType**.

Basically, this uses **Claim** under the covers. So you can verify these **Claims** in your Controllers just as usual.

All these **ClaimTypes** will be used in the token generation.

You can specify multiple Claim Types.

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

            services.AddSecurity<Authenticator, UserModel>(this.Configuration, builder =>
                builder.AddIdType(IdType.Name, userModel => userModel.Id)
                       .AddIdType(IdType.Role, userModel => userModel.Role)
                       .AddIdType("DOB", userModel => userModel.DOB.ToShortDateString())
            , true);
            services.AddMvc().AddSecurity<UserModel>();
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
    "Secret": "a secret that needs to be at least 16 characters long",
    "Issuer": "your app",
    "Audience": "the client of your app",
    "TokenExpiryInHours" :  2
  },
  .
  .
  .
}
```