using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    public interface IIdTypeBuilder<TUserModel>
        where TUserModel : IAuthenticationUser
    {
        IIdTypeBuilder<TUserModel> AddIdType(string type, string value);

        IIdTypeBuilder<TUserModel> AddIdType(IdType idType, string value);

        IIdTypeBuilder<TUserModel> AddIdType(string idType, Func<TUserModel, string> value);

        IIdTypeBuilder<TUserModel> AddIdType(IdType idType, Func<TUserModel, string> value);
    }

    public interface IIdTypeBuilderToClaims
    {
        List<Claim> ToClaims();
    }
}
