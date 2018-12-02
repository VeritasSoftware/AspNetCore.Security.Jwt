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
    /// <typeparam name="TUserModel">The custom User model</typeparam>
    public interface ISecurityService<in TUserModel>
        where TUserModel: class, IAuthenticationUser
    {
        /// <summary>
        /// Generate token from the Claims (specified using the IdTypeBuilder (<see cref="IdTypeBuilder{TUserModel}"/>)
        /// </summary>
        /// <param name="user">The custom User model</param>
        /// <returns>The token</returns>
        string GenerateToken(TUserModel user);
    }
}
