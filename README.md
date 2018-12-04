# AspNetCore.Security.Jwt
## Asp Net Core Jwt Bearer Token Security package.

[**Nuget package**](https://www.nuget.org/packages/AspNetCore.Security.Jwt)

[**Sample API (Microservice) project secured by package**](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt.Microservice)

|Packages|Version & Downloads|
|---------------------------|:---:|
|*AspNetCore.Security.Jwt*|[![NuGet Version and Downloads count](https://buildstats.info/nuget/AspNetCore.Security.Jwt)](https://www.nuget.org/packages/AspNetCore.Security.Jwt)|

![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)
![SonarCloud](https://sonarcloud.io/api/project_badges/quality_gate?project=VeritasSoftware_AspNetCore.Security.Jwt)

[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=alert_status)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=ncloc)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=coverage)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=bugs)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=security_rating)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=sqale_index)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)
[![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=VeritasSoftware_AspNetCore.Security.Jwt&metric=code_smells)](https://sonarcloud.io/dashboard?id=VeritasSoftware_AspNetCore.Security.Jwt)

**The package:**

*	Makes adding **Jwt Bearer Token Security** to your **ASP NET Core 2.0 app** a breeze!!

*	Even gives you an out of the box **TokenController** to issue Jwt tokens. 

*	And integrates the TokenContoller into your app automatically.

*	**Azure Active Directory (AD)** authentication integration.

*	**Google OAuth2** authentication integration.

*	**Facebook** authentication integration.

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

## Azure Active Directory (AD) authentication integration

[**Using Azure AD for authentication**](READMEAzureAD.md)

## Google OAuth2 authentication integration

[**Using Google for authentication**](READMEGoogle.md)

## Facebook authentication integration

[**Using Facebook for authentication**](READMEFacebook.md)

## In your Controller that you want to secure

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

## Swagger UI integration

When you start Swagger you will see a **Token endpoint (/token)** automatically.

Also, you will see an **Authorize** button.

![Swagger Integration](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/SwaggerIntegration.jpg)

You obtain the Jwt token by entering your Id and Password on the Token endpoint.

Then you enter the token into the Value field after clicking on the Authorize button as

![Available Authorizations](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/AvailableAuthorizations.jpg)

Then, you can make calls to all secured endpoints (marked with Authorize attribute).

## Using multiple Authentication

You can use multiple authentications in your app.

*	Default

**OR**

*	Custom User model

**AND**

*	Azure Active Directory

*	Google

*	Facebook

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

            //Default + AzureAD + Google + Facebook
            //services
            //        .AddSecurity(securitySettings, true)
            //        .AddSecurity<Authenticator>()
            //        .AddFacebookSecurity(builder =>
            //            builder.AddClaim("FacebookUser", userModel => userModel.UserAccessToken))
            //        .AddAzureADSecurity()
            //        .AddGoogleSecurity();

            //OR

            //Custom User model auth + Azure AD + Google + Facebook            
            services
                   .AddSecurity(securitySettings, true)
                   .AddSecurity<CustomAuthenticator, UserModel>(builder =>
                       builder.AddClaim(IdType.Name, userModel => userModel.Id)
                              .AddClaim(IdType.Role, userModel => userModel.Role)
                              .AddClaim("DOB", userModel => userModel.DOB.ToShortDateString()))
                   .AddFacebookSecurity(builder =>
                       builder.AddClaim("FacebookUser", userModel => userModel.UserAccessToken.ToString()))
                   .AddAzureADSecurity()
                   .AddGoogleSecurity();

            //services.AddMvc()
            //        .AddSecurity()
            //        .AddFacebookSecurity()
            //        .AddAzureADSecurity()
            //        .AddGoogleSecurity();
			
            //OR

            services.AddMvc()
                    .AddSecurity<UserModel>()
                    .AddFacebookSecurity()
                    .AddAzureADSecurity()
                    .AddGoogleSecurity();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

In Swagger UI, you will see multiple endpoints:

![Multiple Authorizations](https://github.com/VeritasSoftware/AspNetCore.Security.Jwt/blob/master/MultipleAuth.jpg)

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
