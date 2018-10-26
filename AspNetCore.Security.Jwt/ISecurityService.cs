namespace AspNetCore.Security.Jwt
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

    /// <summary>
    /// Generic ISecurityService interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISecurityService<T>
        where T: class, IAuthenticationUser
    {        
        string GenerateToken(T user);
    }
}
