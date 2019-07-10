﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    public interface IIdTypeBuilder
    {
        IIdTypeBuilder AddClaim(string type, string value);

        IIdTypeBuilder AddClaim(IdType idType, string value);
    }

    /// <summary>
    /// IIdTypeBuilder interface. Used for adding IdTypes (ClaimTypes) to the token.
    /// </summary>
    /// <typeparam name="TUserModel">The custom User model</typeparam>
    public interface IIdTypeBuilder<TUserModel>
        where TUserModel : class, IAuthenticationUser
    {
        IIdTypeBuilder<TUserModel> AddClaim(string type, string value);

        IIdTypeBuilder<TUserModel> AddClaim(IdType idType, string value);

        IIdTypeBuilder<TUserModel> AddClaim(string idType, Func<TUserModel, string> value);

        IIdTypeBuilder<TUserModel> AddClaim(IdType idType, Func<TUserModel, string> value);
    }

    /// <summary>
    /// IIdTypeBuilderToClaims interface - Used to return the Claims generated by the IdTypeBuilder (<see cref="IdTypeBuilder{TUserModel}"/>)
    /// </summary>
    public interface IIdTypeBuilderToClaims
    {
        List<Claim> ToClaims();
    }
}
