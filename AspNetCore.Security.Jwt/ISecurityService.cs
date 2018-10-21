namespace Api.Security.Jwt
{
    /// <summary>
    /// ISecurityService interface
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The token</returns>
        string GenerateToken(string seed);       
    }
}
