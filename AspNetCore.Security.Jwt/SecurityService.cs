namespace Api.Security.Jwt
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// SecurityService class
    /// </summary>
    public class SecurityService : ISecurityService
    {
        private readonly SecuritySettings securitySettings;

        public SecurityService(SecuritySettings securitySettings)
        {
            this.securitySettings = securitySettings;
        }
        
        /// <inheritdoc>
        public string GenerateToken(string seed)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.securitySettings.Secret));

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, seed),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            var token = new JwtSecurityToken(
                issuer: this.securitySettings.Issuer,
                audience: this.securitySettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(28),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }

    public class SecuritySettings
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}
