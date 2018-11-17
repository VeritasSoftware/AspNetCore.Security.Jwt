using AspNetCore.Security.Jwt.Facebook;
using System;

namespace AspNetCore.Security.Jwt
{
    public interface IAddSecurityBuilder
    {
        IAddSecurityBuilder AddSecurity<TAuthenticator>(bool addSwaggerSecurity = false)
             where TAuthenticator : class, IAuthentication;

        IAddSecurityBuilder AddSecurity<TAuthenticator, TUserModel>(Action<IIdTypeBuilder<TUserModel>> addClaims = null,
                                                                bool addSwaggerSecurity = false)
            where TAuthenticator : class, IAuthentication<TUserModel>
            where TUserModel : class, IAuthenticationUser;

        IAddSecurityBuilder AddFacebookSecurity(Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null,
                                                                bool addSwaggerSecurity = false);

        IAddSecurityBuilder AddAzureADSecurity(bool addSwaggerSecurity = false);
    }
}
