# AspNetCore.Security.Jwt
Asp Net Core Jwt Bearer Token Security package.

*	Adds Jwt Security to your ASP NET Core 2.0 app in the blink of an eye!!

*	Even gives you an out of the box TokenController to issue Jwt tokens. 

*	And integrates that Contoller into your app automatically.

*	Also, adds Security to Swagger UI!

Add a reference to the package and...

**Implement IAuthentication interface:**
Validate the Username and Password here.

```C#
using AspNetCore.Security.Jwt;

namespace XXX.API
{
    public class Authenticator : IAuthentication
    {
        public bool IsValidUser(string userName, string password)
        {
            //Put your user authenication here.
            return true;
        }
    }
}
```

**In your Startup.cs:**

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

            services.AddSecurity<Authenticator>(this.Configuration, true);
            services.AddMvc().AddSecurity();
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

**And in your appsettings.json:**

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

The Token Contoller has a POST Method which you can call with a Username and Password.
A Jwt Bearer token is then issued which must be sent subsequent requests to the API.

You can mark the Controller or Action that you want to secure with Authorize attribute like:

```C#
using Microsoft.AspNetCore.Mvc;
.
.
.

namespace XXX.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    [Route("api/[controller]")]
    public class XXXController : Controller
    {
		.
		.
		.
    }
}
```
Once this is done,

you have to send the issued Jwt token in the header of the request as

Authorization: Bearer \<token\>

Eg.

Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU2hhbiIsImV4cCI6MTU0MjUxMTkzOSwibmJmIjoxNTQwMDkyNzM5LCJpc3MiOiJ5b3VyIGFwcCIsImF1ZCI6InRoZSBjbGllbnQgb2YgeW91ciBhcHAifQ.VktS3XGD-Z3-wNuXl4IuLLJXe9OUNK5RZ8o-9eUUVuE

In Angular 2+ app, you can do this using HttpInterceptors.

