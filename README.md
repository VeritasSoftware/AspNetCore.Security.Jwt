# AspNetCore.Security.Jwt
## Asp Net Core Jwt Bearer Token Security package.

[**Nuget package**](https://www.nuget.org/packages/AspNetCore.Security.Jwt)

**The package:**

*	Makes adding **Jwt Bearer Token Security** to your **ASP NET Core 2.0 app** a breeze!!

*	Even gives you an out of the box **TokenController** to issue Jwt tokens. 

*	And integrates the TokenContoller into your app automatically.

*	Facebook integration.

*	Also, **Swagger UI** integration!

**Add a reference to the package and...**

## A. Use custom User model authentication

If you want to use a **custom User model** in your app for authentication then do the following for below **steps 1, 2, 3**:

[**Using a custom User model for authentication**](READMECustomUserModel.md)

**OR**

## B. Use Default authentication (Id, Password)

## 1. Implement IAuthentication interface in your app

Validate the Id and Password here.

See **Appendix A** for the **IdType** supported.

After this validation, the Jwt token is issued by the **TokenController**.

```C#
	using AspNetCore.Security.Jwt;
	using System.Threading.Tasks;

	namespace XXX.API
	{
		public class Authenticator : IAuthentication
		{        
			public async Task<bool> IsValidUser(string id, string password)
			{
				//Put your id authenication here.
				return true;
			}
		}
	}
```

The Authenticator is automatically wired up for dependency injection (Scoped).

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

## 3. In your appsettings.json

**Note:-** You can put these settings in **Secret Manager** by using **Manage User Secrets** menu (right-click your Project).

```javascript
{
  "SecuritySettings": {
    "Secret": "a secret that needs to be at least 16 characters long",
    "Issuer": "your app",
    "Audience": "the client of your app",
    "IdType":  "Name",
    "TokenExpiryInHours" :  2
  },
  .
  .
  .
}
```
## 4. In your Controller that you want to secure

You must mark the **Controller or Action** that you want to secure with **Authorize attribute** like:

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

## TokenController - Issues the Jwt token

The **TokenContoller** has a POST Method (/token) which you can call with a Id and Password.

The Id has to match the specified **IdType**.

The POST in **Postman** is like below:

![POST to TokenController](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/TokenRequest.jpg)

A Jwt Bearer token is then issued which must be sent in subsequent requests in the header.

# Access your secure Controller or Action

You have to send the issued Jwt token in the header of the request as

Authorization: Bearer \<token\>

Eg.

Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU2hhbiIsImV4cCI6MTU0MjUxMTkzOSwibmJmIjoxNTQwMDkyNzM5LCJpc3MiOiJ5b3VyIGFwcCIsImF1ZCI6InRoZSBjbGllbnQgb2YgeW91ciBhcHAifQ.VktS3XGD-Z3-wNuXl4IuLLJXe9OUNK5RZ8o-9eUUVuE

to access the Controller or Action.

This is like below in Postman:

![POST to secure API](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/AuthorizationHeader.jpg)

In Angular 2+ app, you can do this using **HttpInterceptors**.

## Facebook integration

[**Using Facebook for authentication**](READMEFacebook.md)

## Swagger UI integration

When you start Swagger you will see a **Token endpoint (/token)** automatically.

Also, you will see an **Authorize** button.

![Swagger Integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/SwaggerIntegration.jpg)

You obtain the Jwt token by entering your Id and Password on the Token endpoint.

Then you enter the token into the Value field after clicking on the Authorize button as

![Available Authorizations](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/AvailableAuthorizations.jpg)

Then, you can make calls to all secured endpoints (marked with Authorize attribute).

## Appendix A

**IdType** is an enum.

All **ClaimTypes** are available via this enum.

The **IdType** your app will use is specified in the **SecuritySettings** of your **appsettings.json**.

If you want to use multiple (or custom IdType) you can specify IdTypes in your SecuritySettings

That can be any one of the ones specified in the enum below: 

```C#
    public enum IdType
    {
        Actor,
        PostalCode,
        PrimaryGroupSid,
        PrimarySid,
        Role,
        Rsa,
        SerialNumber,
        Sid,
        Spn,
        StateOrProvince,
        StreetAddress,
        Surname,
        System,
        Thumbprint,
        Upn,
        Uri,
        UserData,
        Version,
        Webpage,
        WindowsAccountName,
        WindowsDeviceClaim,
        WindowsDeviceGroup,
        WindowsFqbnVersion,
        WindowsSubAuthority,
        OtherPhone,
        NameIdentifier,
        Name,
        MobilePhone,
        Anonymous,
        Authentication,
        AuthenticationInstant,
        AuthenticationMethod,
        AuthorizationDecision,
        CookiePath,
        Country,
        DateOfBirth,
        DenyOnlyPrimaryGroupSid,
        DenyOnlyPrimarySid,
        DenyOnlySid,
        WindowsUserClaim,
        DenyOnlyWindowsDeviceGroup,
        Dsa,
        Email,
        Expiration,
        Expired,
        Gender,
        GivenName,
        GroupSid,
        Hash,
        HomePhone,
        IsPersistent,
        Locality,
        Dns,
        X500DistinguishedName
    }
```
