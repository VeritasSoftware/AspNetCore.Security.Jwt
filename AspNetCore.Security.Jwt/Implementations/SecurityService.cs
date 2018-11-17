namespace AspNetCore.Security.Jwt
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// SecurityService class
    /// </summary>
    internal class SecurityService : ISecurityService
    {
        private readonly SecuritySettings securitySettings;
        const double DEFAULT_TOKEN_EXPIRY_IN_HOURS = 1;

        public SecurityService(SecuritySettings securitySettings)
        {
            this.securitySettings = securitySettings;
        }        

        /// <inheritdoc>
        public string GenerateToken(string seed)
        {
            if (string.IsNullOrEmpty(seed))
                throw new ArgumentNullException(nameof(seed));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.securitySettings.Secret));

            var idType = this.securitySettings.IdType;

            //dynamically generate ClaimTypes from IdType
            //create claim 
            Claim claim = new Claim(idType.ToClaimTypes(), seed);

            var claims = new Claim[] {
                claim,
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddHours(this.securitySettings.TokenExpiryInHours ?? DEFAULT_TOKEN_EXPIRY_IN_HOURS)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            var token = new JwtSecurityToken(
                issuer: this.securitySettings.Issuer,
                audience: this.securitySettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(this.securitySettings.TokenExpiryInHours ?? DEFAULT_TOKEN_EXPIRY_IN_HOURS),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }

    /// <summary>
    /// Generic SecurityService class
    /// </summary>
    /// <typeparam name="TUserModel">The User Model</typeparam>
    internal class SecurityService<TUserModel> : ISecurityService<TUserModel>
        where TUserModel: class, IAuthenticationUser
    {
        private readonly BaseSecuritySettings securitySettings;
        private Action<IIdTypeBuilder<TUserModel>> addClaims;
        const double DEFAULT_TOKEN_EXPIRY_IN_HOURS = 1;        

        public SecurityService(BaseSecuritySettings securitySettings, Action<IIdTypeBuilder<TUserModel>> addClaims = null)
        {
            this.securitySettings = securitySettings;
            this.addClaims = addClaims;
        }

        public string GenerateToken(TUserModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.securitySettings.Secret));

            var builder = new IdTypeBuilder<TUserModel>(user);

            if (addClaims != null)
            {
                this.addClaims(builder);
            }

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddHours(this.securitySettings.TokenExpiryInHours ?? DEFAULT_TOKEN_EXPIRY_IN_HOURS)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            if (addClaims != null)
            {
                claims.AddRange(builder.ToClaims());
            }

            var token = new JwtSecurityToken(
                issuer: this.securitySettings.Issuer,
                audience: this.securitySettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(this.securitySettings.TokenExpiryInHours ?? DEFAULT_TOKEN_EXPIRY_IN_HOURS),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
