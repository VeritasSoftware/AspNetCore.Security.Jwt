using AspNetCore.Security.Jwt.Facebook;
using System;

namespace AspNetCore.Security.Jwt
{
    public interface IAddSecurityBuilder
    {
        IAddSecurityBuilder AddSecurity<TAuthenticator>()
             where TAuthenticator : class, IAuthentication;

        IAddSecurityBuilder AddSecurity<TAuthenticator, TUserModel>(Action<IIdTypeBuilder<TUserModel>> addClaims = null)
            where TAuthenticator : class, IAuthentication<TUserModel>
            where TUserModel : class, IAuthenticationUser;

        IAddSecurityBuilder AddFacebookSecurity(Action<IIdTypeBuilder<FacebookAuthModel>> addClaims = null);

        IAddSecurityBuilder AddAzureADSecurity();
    }
}
