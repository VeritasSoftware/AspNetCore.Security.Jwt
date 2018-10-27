using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    /// <inheritdoc />
    public class IdTypeBuilder<TUserModel> : IIdTypeBuilder<TUserModel>, IIdTypeBuilderToClaims
        where TUserModel : IAuthenticationUser
    {
        private List<Claim> claims = new List<Claim>();
        TUserModel user;

        public IdTypeBuilder(TUserModel user)
        {
            this.user = user;
        }

        public IIdTypeBuilder<TUserModel> AddClaim(string type, string value)
        {
            claims.Add(new Claim(type, value));

            return this;
        }

        public IIdTypeBuilder<TUserModel> AddClaim(IdType idType, string value)
        {
            claims.Add(new Claim(idType.ToClaimTypes(), value));

            return this;
        }

        public IIdTypeBuilder<TUserModel> AddClaim(string idType, Func<TUserModel, string> value)
        {
            claims.Add(new Claim(idType, value(user)));

            return this;
        }

        public IIdTypeBuilder<TUserModel> AddClaim(IdType idType, Func<TUserModel, string> value)
        {
            claims.Add(new Claim(idType.ToClaimTypes(), value(user)));

            return this;
        }

        public List<Claim> ToClaims()
        {
            return this.claims;
        }
    }
}
